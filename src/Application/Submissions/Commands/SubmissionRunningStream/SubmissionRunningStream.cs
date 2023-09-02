using System.Runtime.CompilerServices;
using Code_Judge.Application.Common.Exceptions;
using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Application.Common.Models;
using Code_Judge.Application.Problems.Queries.GetProblemById;
using Code_Judge.Application.Submissions.Commands.CreateSubmission;
using Code_Judge.Application.TestCases.Queries.GetTestCases;
using Code_Judge.Domain.Entities;
using Code_Judge.Domain.Enums;
using MediatR;

namespace Code_Judge.Application.Submissions.Commands.SubmissionRunningStream;

public class SubmissionRunningStream : IStreamRequest<ExecuteCodeResult>
{
    public int ProblemId { get; init; }
    public string Code { get; init; } = null!;
    public ProgramingLanguage Language { get; init; }
}

public class SubmissionRunningStreamHandler : IStreamRequestHandler<SubmissionRunningStream, ExecuteCodeResult>
{
    private readonly IExecuteCodeStrategyFactory _executeCodeStrategyFactory;
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;

    public SubmissionRunningStreamHandler(IExecuteCodeStrategyFactory executeCodeStrategyFactory, IMediator mediator,
        IApplicationDbContext context)
    {
        _executeCodeStrategyFactory = executeCodeStrategyFactory;
        _mediator = mediator;
        _context = context;
    }

    public async IAsyncEnumerable<ExecuteCodeResult> Handle(SubmissionRunningStream request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var executeCodeStrategy = _executeCodeStrategyFactory.GetExecuteCodeStrategy(request.Language);
        var listTestCases = await _mediator.Send(new GetTestCasesQuery(request.ProblemId), cancellationToken);
        var problem = await _mediator.Send(new GetProblemByIdQuery(request.ProblemId), cancellationToken);
        if (problem is null)
        {
            throw new NotFoundException(nameof(Problem), request.ProblemId);
        }

        var compileResult = await executeCodeStrategy.CompileCodeAsync(request.Code, cancellationToken);
        if (!compileResult.IsSuccess)
        {
            var newSubmission = new Submission
            {
                Code = request.Code,
                Language = request.Language,
                ProblemId = request.ProblemId,
                Memory = 0,
                RunTime = 0,
                Status = SubmissionStatus.CompileError,
                Error = compileResult.Error,
                
            };
            await _context.Submissions.AddAsync(newSubmission,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            foreach (var testCase in listTestCases)
            {
                yield return new ExecuteCodeResult
                {
                    Status = SubmissionStatus.CompileError,
                    TestCase = testCase,
                    Error = compileResult.Error,
                    IsSuccess = false,
                    ExitCode = 1,
                };
            }
        }

        var executeCodeTasks = listTestCases
            .Select(testCase => Execute(executeCodeStrategy, compileResult.FileName, testCase, problem)).ToList();
        var createSubmissionTask = CreateSubmissions(executeCodeTasks, request, cancellationToken);
        while (cancellationToken.IsCancellationRequested == false)
        {
            var executeCodeTask = await Task.WhenAny(executeCodeTasks);
            var executeCodeResult = await executeCodeTask;
            yield return executeCodeResult;
            executeCodeTasks = executeCodeTasks.Where(x => x != executeCodeTask).ToList();
            if (executeCodeTasks.Count == 0)
            {
                await createSubmissionTask;
                break;
            }
        }
    }

    private async Task CreateSubmissions(IEnumerable<Task<ExecuteCodeResult>> executeCodeTasks, SubmissionRunningStream request, CancellationToken cancellationToken)
    {
        var result= await  Task.WhenAll(executeCodeTasks);
        var averageTime = result.Average(x => x.TimeElapsed);
        var averageMemory = result.Average(x => x.MemoryUsage);
        var status = result.All(x => x.Status ==SubmissionStatus.Accepted) ? SubmissionStatus.Accepted : SubmissionStatus.Rejected;
        status = result.Any(x => x.Status == SubmissionStatus.TimeLimitExceeded) ? SubmissionStatus.TimeLimitExceeded : status;
        status = result.Any(x => x.Status == SubmissionStatus.MemoryLimitExceeded) ? SubmissionStatus.MemoryLimitExceeded : status;
        status = result.Any(x => x.Status == SubmissionStatus.RuntimeError) ? SubmissionStatus.RuntimeError : status;
        status = result.Any(x => x.Status == SubmissionStatus.CompileError) ? SubmissionStatus.CompileError : status;

        var submission = new Submission
        {
            Code = request.Code,
            Language = request.Language,
            ProblemId = request.ProblemId,
            Memory = averageMemory,
            RunTime = (int)Math.Ceiling(averageTime),
            Status =  status,
        };
        await  _context.Submissions.AddAsync(submission,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

    }
    private async Task<ExecuteCodeResult> Execute(IExecuteCodeStrategy executeCodeStrategy, string fileName,
        TestCase testCase, Problem problem)
    {
        var executeCodeResult = await executeCodeStrategy.ExecuteAsync(fileName, testCase.Input, testCase.Output,
            problem.TimeLimit, problem.MemoryLimit);
        executeCodeResult.TestCase = testCase;
        return executeCodeResult;
    }
}
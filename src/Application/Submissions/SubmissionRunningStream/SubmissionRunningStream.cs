using System.Runtime.CompilerServices;
using Code_Judge.Application.Common.Exceptions;
using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Application.Common.Models;
using Code_Judge.Application.Problems.Queries.GetProblemById;
using Code_Judge.Application.TestCases.Queries.GetTestCases;
using Code_Judge.Domain.Entities;
using Code_Judge.Domain.Enums;
using MediatR;

namespace Code_Judge.Application.Submissions.SubmissionRunningStream;

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

        while (cancellationToken.IsCancellationRequested == false)
        {
            var executeCodeTask = await Task.WhenAny(executeCodeTasks);
            var executeCodeResult = await executeCodeTask;
            yield return executeCodeResult;
            executeCodeTasks = executeCodeTasks.Where(x => x != executeCodeTask).ToList();
            if (executeCodeTasks.Count == 0)
            {
                break;
            }
        }
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
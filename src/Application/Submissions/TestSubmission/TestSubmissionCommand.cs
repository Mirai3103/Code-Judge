using Code_Judge.Application.Common.Exceptions;
using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Application.Common.Models;
using Code_Judge.Application.Problems.Queries.GetProblemById;
using Code_Judge.Application.TestCases.Queries.GetTestCases;
using Code_Judge.Domain.Entities;
using Code_Judge.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;

namespace Code_Judge.Application.Submissions.TestSubmission;

public record TestSubmissionCommand:IRequest<IEnumerable<ExecuteCodeResult>>
{
    public int ProblemId { get; init; }
    public string Code { get; init; } = null!;
    public ProgramingLanguage Language { get; init; }
}

public class TestSubmissionCommandHandler : IRequestHandler<TestSubmissionCommand, IEnumerable<ExecuteCodeResult>>
{
    private readonly IExecuteCodeStrategyFactory _executeCodeStrategyFactory;
    private readonly IMediator _mediator;

    public TestSubmissionCommandHandler(IExecuteCodeStrategyFactory executeCodeStrategyFactory, IMediator mediator)
    {
        _executeCodeStrategyFactory = executeCodeStrategyFactory;
        _mediator = mediator;
    }

    public async Task<IEnumerable<ExecuteCodeResult>> Handle(TestSubmissionCommand request, CancellationToken cancellationToken)
    {
        var executeCodeStrategy = _executeCodeStrategyFactory.GetExecuteCodeStrategy(request.Language);
        var listPublishTestCases = await _mediator.Send(new GetPublishTestCasesQuery(request.ProblemId),cancellationToken);
        var problem = await _mediator.Send(new GetProblemByIdQuery(request.ProblemId),cancellationToken);
        if (problem is null)
        {
            throw new NotFoundException(nameof(Problem),request.ProblemId);
        }
        var compileResult = await executeCodeStrategy.CompileCodeAsync(request.Code,cancellationToken);
        if (!compileResult.IsSuccess)
        {
            return listPublishTestCases.Select(testCase => new ExecuteCodeResult
            {
                Status = SubmissionStatus.CompileError,
                MemoryUsage = 0,
                TimeElapsed = 0,
                IsSuccess = false,
                Error = "Compile Error",
                ExitCode = -1,
                TestCase = testCase
            });
        }
        var executeCodeTasks = listPublishTestCases.Select(testCase => Execute(executeCodeStrategy,compileResult.FileName,testCase,problem));
   
        return   await Task.WhenAll(executeCodeTasks);
        
    }
    private async Task<ExecuteCodeResult> Execute(IExecuteCodeStrategy executeCodeStrategy,string fileName,TestCase testCase,Problem problem)
    {
        var executeCodeResult = await executeCodeStrategy.ExecuteAsync(fileName,testCase.Input,testCase.Output,problem.TimeLimit,problem.MemoryLimit);
        executeCodeResult.TestCase = testCase;
        return executeCodeResult;
    }
}
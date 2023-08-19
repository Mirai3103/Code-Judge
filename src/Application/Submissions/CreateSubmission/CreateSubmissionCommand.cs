﻿using Code_Judge.Application.Common.Exceptions;
using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Application.Common.Models;
using Code_Judge.Application.Problems.Queries.GetProblemById;
using Code_Judge.Application.TestCases.Queries.GetTestCases;
using Code_Judge.Domain.Entities;
using Code_Judge.Domain.Enums;
using MediatR;

namespace Code_Judge.Application.Submissions.CreateSubmission;

public record CreateSubmissionCommand:IRequest<IEnumerable<ExecuteCodeResult>>
{
    public int ProblemId { get; init; }
    public string Code { get; init; } = null!;
    public ProgramingLanguage Language { get; init; }
}

public class CreateSubmissionCommandHandler : IRequestHandler<CreateSubmissionCommand, IEnumerable<ExecuteCodeResult>>
{
    private readonly IExecuteCodeStrategyFactory _executeCodeStrategyFactory;
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    public CreateSubmissionCommandHandler(IExecuteCodeStrategyFactory executeCodeStrategyFactory, IMediator mediator,IApplicationDbContext context)
    {
        _executeCodeStrategyFactory = executeCodeStrategyFactory;
        _mediator = mediator;
        _context = context;
    }

    public async Task<IEnumerable<ExecuteCodeResult>> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken)
    {
        var executeCodeStrategy = _executeCodeStrategyFactory.GetExecuteCodeStrategy(request.Language);
        var listTestCases = await _mediator.Send(new GetTestCasesQuery(request.ProblemId),cancellationToken);
        var problem = await _mediator.Send(new GetProblemByIdQuery(request.ProblemId),cancellationToken);
        if (problem is null)
        {
            throw new NotFoundException(nameof(Problem),request.ProblemId);
        }
        var compileResult = await executeCodeStrategy.CompileCodeAsync(request.Code,cancellationToken);
        if (!compileResult.IsSuccess)
        {
            return listTestCases.Select(testCase => new ExecuteCodeResult
            {
                Status = SubmissionStatus.CompileError,
                MemoryUsage = 0,
                TimeElapsed = 0,
                IsSuccess = false,
                Error = "Compile Error",
                ExitCode = -1
            });
        }
        var executeCodeTasks = listTestCases.Select(testCase => Execute(executeCodeStrategy,compileResult.FileName,testCase,problem));
   
      var result= await Task.WhenAll(executeCodeTasks);
        var averageTime =(int) result.Average(x => x.TimeElapsed);
        var averageMemory = result.Average(x => x.MemoryUsage);
        var status = result.All(x => x.Status ==SubmissionStatus.Accepted) ? SubmissionStatus.Accepted : SubmissionStatus.Rejected;
        status = result.Any(x => x.Status == SubmissionStatus.RuntimeError) ? SubmissionStatus.RuntimeError : status;
        status = result.Any(x => x.Status == SubmissionStatus.CompileError) ? SubmissionStatus.CompileError : status;
     
        var submission = new Submission
        {
            Code = request.Code,
            Language = request.Language,
            ProblemId = request.ProblemId,
            Memory = averageMemory,
            RunTime = averageTime,
            Status =  status,
        };
       await  _context.Submissions.AddAsync(submission,cancellationToken);
       return result;
    }
    private async Task<ExecuteCodeResult> Execute(IExecuteCodeStrategy executeCodeStrategy,string fileName,TestCase testCase,Problem problem)
    {
    
        var executeCodeResult = await executeCodeStrategy.ExecuteAsync(fileName,testCase.Input,testCase.Output,problem.TimeLimit,problem.MemoryLimit);
        executeCodeResult.TestCase = testCase;
        return executeCodeResult;
    }
}
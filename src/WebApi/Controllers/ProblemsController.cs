using Code_Judge.Application.Common.Models;
using Code_Judge.Application.Problems.Commands.CreateProblem;
using Code_Judge.Application.Problems.Queries.GetProblemsWithPagination;
using Code_Judge.Application.TestCases.Queries.GetTestCases;
using Code_Judge.Domain.Entities;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Code_Judge.WebUI.Controllers;

public class ProblemsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedList<ProblemBriefDto>>> GetProblemsWithPagination([FromQuery] GetProblemsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }
    [Authorize()]
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateProblemCommand command)
    {
        var user =  HttpContext.User;
        return await Mediator.Send(command);
    }
    [HttpGet("{id}/TestCases")]
    public async Task<ActionResult<IEnumerable<TestCase>>> GetTestCases(int id)
    {
        var query = new GetTestCasesQuery(id);
        return Ok(await Mediator.Send(query));
    }
    [HttpGet("{id}/PublishTestCases")]
    public async Task<ActionResult<IEnumerable<TestCase>>> GetPublishTestCases(int id)
    {
        var query = new GetPublishTestCasesQuery(id);
        return Ok(await Mediator.Send(query));
    }
}
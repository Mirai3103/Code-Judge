using Code_Judge.Application.TestCases.Commands.CreateTestCase;
using Microsoft.AspNetCore.Mvc;

namespace Code_Judge.WebUI.Controllers;

public class TestCasesController: ApiControllerBase
{
    [HttpPost] 
    public async Task<ActionResult<int>> Create(CreateTestCaseCommand command)
    {
        return await Mediator.Send(command);
    }
}

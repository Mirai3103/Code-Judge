using Code_Judge.Application.Common.Exceptions;
using Code_Judge.Application.Problems.Commands.CreateProblem;
using Code_Judge.Application.TestCases.Commands.CreateTestCase;
using Code_Judge.Domain.Entities;
using Code_Judge.Domain.Enums;
using FluentAssertions;
using NUnit.Framework;


namespace Code_Judge.Application.IntegrationTests.TestCases.Commands;

public class CreateTestCaseTest:BaseTestFixture
{
    [Test]
    public  Task ShouldRequireMinimumFields()
    {
        var command = new CreateTestCaseCommand(){};
        Assert.ThrowsAsync<ValidationException>(async () => await Testing.SendAsync(command));
        return Task.CompletedTask;
    }
    [Test]
    public Task ShouldNotFoundProblem()
    {
        var command = new CreateTestCaseCommand()
        {
            ProblemId = 99999999,
            Input = "input",
            Output = "output"
        };
        Assert.ThrowsAsync<NotFoundException>(async () => await Testing.SendAsync(command));
        return Task.CompletedTask;
    }
    [Test]
    public async Task ShouldCreateTestCaseSuccessfully()
    {
        var problemId = await Testing.SendAsync(new CreateProblemCommand()
        {
            Description = "descriptiosadasdsadassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssadsadasdasdsadasdasdasdn",
            TimeLimit = 1,
            MemoryLimit = 1,
            Hint = "hint",
           DifficultyLevel = DifficultyLevel.Hard,
           IsPublic = true,
           Name = "namesadasdsadasdasd",
           Points = 1,
        });
        
        var command = new CreateTestCaseCommand()
        {
            ProblemId = problemId,
            Input = "input",
            Output = "output",IsHidden =  false
        };
        var id = await Testing.SendAsync(command);
        var testCase = await Testing.FindAsync<TestCase>(id);
        testCase.Should().NotBeNull();
        testCase!.Input.Should().Be(command.Input);
        testCase.Output.Should().Be(command.Output);
        testCase.ProblemId.Should().Be(command.ProblemId);
    }
}
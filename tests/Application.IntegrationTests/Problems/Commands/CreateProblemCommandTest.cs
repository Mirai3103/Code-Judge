using Code_Judge.Application.Common.Exceptions;
using Code_Judge.Application.Problems.Commands.CreateProblem;
using Code_Judge.Application.TodoItems.Commands.CreateTodoItem;
using Code_Judge.Application.TodoLists.Commands.CreateTodoList;
using Code_Judge.Domain.Entities;
using FluentAssertions;
using Code_Judge.Application.Problems.Commands.CreateProblem;
using Code_Judge.Domain.Enums;
using NUnit.Framework;

namespace Code_Judge.Application.IntegrationTests.Problems.Commands;

using static Testing;

public class CreateProblemCommandTest : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateProblemCommand();
    
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }


    [Test]
    public async Task ShouldCreateProblem()
    {
        var userId = await RunAsDefaultUserAsync();
        var command = new CreateProblemCommand()
        {
            MemoryLimit = 10,
            TimeLimit = 1000,
            DifficultyLevel = DifficultyLevel.Easy,
            TemplateCode =
                "function twoSum(nums: number[], target: number): number[] {\n    const object: any = {};\n\n    for (let i = 0; i < nums.length; i++) {\n        let need = target - nums[i];\n        if (object[need] !== undefined) {\n            return [object[need], i];\n        } else {\n            object[nums[i]] = i;\n        }\n    }\n    return [0, 0];\n};",
            Description =
                "Given an array of integers nums and an integer target, return indices of the two numbers such that they add up to target.",
            IsPublic = true,
            Name = "Two Sum",
            Points = 100,
        };
        var problemId = await SendAsync(command);


        var item = await FindAsync<Problem>(problemId);

        item.Should().NotBeNull();
        item!.Name.Should().Be(command.Name);
        item.CreatedBy.Should().Be(userId);
        item.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
        item.ContestId.Should().Be(null);
        item.Description.Should().Be(command.Description);
        item.DifficultyLevel.Should().Be(command.DifficultyLevel);
        item.Hint.Should().Be(null);
        item.IsPublic.Should().Be(command.IsPublic);
        item.MemoryLimit.Should().Be(command.MemoryLimit);
        item.Points.Should().Be(command.Points);
        item.TimeLimit.Should().Be(command.TimeLimit);
        item.TemplateCode.Should().Be(command.TemplateCode);
    }
}
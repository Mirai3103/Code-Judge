using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Domain.Enums;
using Code_Judge.Infrastructure.ExecuteCode;
using NUnit.Framework;

namespace Code_Judge.Application.IntegrationTests.ExecuteCode;

public class ExecutePythonTest
{
    private readonly IExecuteCodeStrategy _executeCppStrategy = Testing.GetService<ExecutePythonStrategy>();
    
    [Test]
    public async Task ShouldSuccess()
    {
        var code = @"print(""Hello World!"")";
        var input = @"1 2";
        var expectedOutput = "Hello World!\r\n";
        var timeLimit = 1000;
        var memoryLimit = 20;
        var compileResult = await _executeCppStrategy.CompileCodeAsync(code);
        Assert.IsTrue(compileResult.IsSuccess);
        var result = await _executeCppStrategy.ExecuteAsync(compileResult.FileName, input, expectedOutput, timeLimit, memoryLimit);
        Assert.AreEqual(SubmissionStatus.Accepted, result.Status);
    }
    [Test]
    public async Task ShouldRuntimeError()
    {
        var code = @"print(1/0)";
        var input = @"1 2";
        var expectedOutput = "Hello World!\r\n";
        var timeLimit = 1000;
        var memoryLimit = 20;
        var compileResult = await _executeCppStrategy.CompileCodeAsync(code);
        Assert.IsTrue(compileResult.IsSuccess);
        var result = await _executeCppStrategy.ExecuteAsync(compileResult.FileName, input, expectedOutput, timeLimit, memoryLimit);
        Assert.AreEqual(SubmissionStatus.RuntimeError, result.Status);
    }
    [Test]
    public async Task ShouldTimeLimitExceeded()
    {
        var code = @"while True: pass";
        var input = @"1 2";
        var expectedOutput = "Hello World!\r\n";
        var timeLimit = 1000;
        var memoryLimit = 20;
        var compileResult = await _executeCppStrategy.CompileCodeAsync(code);
        Assert.IsTrue(compileResult.IsSuccess);
        var result = await _executeCppStrategy.ExecuteAsync(compileResult.FileName, input, expectedOutput, timeLimit, memoryLimit);
        Assert.AreEqual(SubmissionStatus.TimeLimitExceeded, result.Status);
    }
    [Test]
    public async Task ShouldReadListOfString()
    {
        var code = "list=[]\r\nfor i in range(2):\r\n\tlist.append(input())\r\nprint(list)";
        var input = "1 2\r\n3 4";
        var expectedOutput = "[\'1 2\', \'3 4\']\r\n";
        var timeLimit = 1000;
        var memoryLimit = 20;
        var compileResult = await _executeCppStrategy.CompileCodeAsync(code);
        Assert.IsTrue(compileResult.IsSuccess);
        var result = await _executeCppStrategy.ExecuteAsync(compileResult.FileName, input, expectedOutput, timeLimit, memoryLimit);
        Assert.AreEqual(SubmissionStatus.Accepted, result.Status);
    }
}
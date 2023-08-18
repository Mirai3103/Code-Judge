using System.Diagnostics;
using Code_Judge.Application.Common.Models;
using Microsoft.Extensions.Configuration;

namespace Code_Judge.Infrastructure.ExecuteCode;

public class ExecutePythonStrategy:BaseExecuteCodeStrategy
{    private readonly IConfiguration _configuration;
    private const string FileExtension = ".py";
    private readonly string _executeCodePath;

    public ExecutePythonStrategy(IConfiguration configuration)
    {
        _configuration = configuration;
        var os = Environment.OSVersion;
        _executeCodePath = _configuration["ExecuteCodePath"] ?? throw new InvalidOperationException();
    }
    public override Task<ExecuteCodeResult> ExecuteCodeAsync(string code, string input, string expectedOutput, int timeLimit, float memoryLimit)
    {
        var executionFilename = Guid.NewGuid().ToString()+FileExtension;
        var fileExecutePath = Path.Combine(_executeCodePath, executionFilename);
        File.WriteAllText(fileExecutePath,code);
        Process process = new ()
        {
            StartInfo =
            {
                FileName = "python3",
                Arguments = fileExecutePath,
                WorkingDirectory = _executeCodePath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                
            },
        };
        return WatchProcess(process,input,expectedOutput,timeLimit,memoryLimit);
    }
}
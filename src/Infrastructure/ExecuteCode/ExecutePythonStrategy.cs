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
    public override Task<ExecuteCodeResult> ExecuteAsync(string fileName, string input, string expectedOutput, int timeLimit, float memoryLimit, CancellationToken cancellationToken = default)
    {
    
        Process process = new ()
        {
            StartInfo =
            {
                FileName = "python3",
                Arguments = fileName+FileExtension,
                WorkingDirectory = _executeCodePath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                
            },
        };
        return WatchProcess(process,input,expectedOutput,timeLimit,memoryLimit, cancellationToken);
    }

    public override async Task<CompileResult> CompileCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var compilationResult = new CompileResult()
        {
            IsSuccess = true,
        };
        var filePath = Path.Combine(_executeCodePath, compilationResult.FileName + FileExtension);
        await File.WriteAllTextAsync(filePath, code, cancellationToken);
        return compilationResult;
    }
}
using System.Diagnostics;
using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Application.Common.Models;
using Code_Judge.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace Code_Judge.Infrastructure.ExecuteCode;

public class ExecuteCppStrategy : BaseExecuteCodeStrategy
{
    private readonly IConfiguration _configuration;
    private const string FileExtension = ".cpp";
    private readonly string _executeFileExtension;
    private readonly string _executeCodePath;

    public ExecuteCppStrategy(IConfiguration configuration)
    {
        _configuration = configuration;
        var os = Environment.OSVersion;
        _executeFileExtension = os.Platform == PlatformID.Unix ? ".out" : ".exe";
        _executeCodePath = _configuration["ExecuteCodePath"] ?? throw new InvalidOperationException();
    }

    public override async Task<ExecuteCodeResult> ExecuteAsync(string fileName, string input, string expectedOutput,
        int timeLimit, float memoryLimit, CancellationToken cancellationToken = default)
    {
      
        // todo: execute 
        var fileExecutePath = Path.Combine(_executeCodePath, fileName + _executeFileExtension);
       
        var process = new Process()
        {
            StartInfo =
            {
                FileName = fileExecutePath,
                Arguments = "",
                WorkingDirectory = _executeCodePath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                
            },
        };
        return await WatchProcess(process,input,expectedOutput,timeLimit,memoryLimit);
    }
    public override async Task<CompileResult> CompileCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var compilationResult = new CompileResult()
        {
            IsSuccess = true,
        };
        var filePath = Path.Combine(_executeCodePath, compilationResult.FileName + FileExtension);
        await File.WriteAllTextAsync(filePath, code, cancellationToken);
        var process = new Process()
        {
            StartInfo =
            {
                FileName = "g++",
                Arguments =
                    $"{compilationResult.FileName + FileExtension} -o {compilationResult.FileName}{_executeFileExtension}",
                WorkingDirectory = _executeCodePath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        process.Start();
     
        await process.WaitForExitAsync(cancellationToken);
        if (process.ExitCode != 0)
        {
            compilationResult.IsSuccess = false;
        }

        return compilationResult;
    }
}
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

    public override async Task<ExecuteCodeResult> ExecuteCodeAsync(string code, string input, string expectedOutput,
        int timeLimit, float memoryLimit)
    {
        var executionFilename = Guid.NewGuid().ToString();
        var isSuccessful = await CompileCodeAsync(code, executionFilename);
        if (!isSuccessful)
        {
            return new ExecuteCodeResult()
            {
                ExitCode = 1,
                Error = "Compilation Error",
                Status = SubmissionStatus.CompileError,
                IsSuccess = false,
                MemoryUsage = 0,
                TimeElapsed = 0,
            };
        }

        // todo: execute 
        var fileExecutePath = Path.Combine(_executeCodePath, executionFilename + _executeFileExtension);
       
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
    private async Task<bool> CompileCodeAsync(string code, string executionFilename)
    {
        var filePath = Path.Combine(_executeCodePath, executionFilename + FileExtension);
        await File.WriteAllTextAsync(filePath, code);
        var process = new Process()
        {
            StartInfo =
            {
                FileName = "g++",
                Arguments =
                    $"{executionFilename + FileExtension} -o {executionFilename}{_executeFileExtension}",
                WorkingDirectory = _executeCodePath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        process.Start();
     
        await process.WaitForExitAsync();
        if (process.ExitCode != 0)
        {
            return false;
        }

        return true;
    }
}
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
        var maxMemoryUsageInBytes = (long)(memoryLimit * 1024 * 1024);
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
        var result = new ExecuteCodeResult();
        process.Start();
        await process.StandardInput.WriteLineAsync(input);
        var memoryUsageTask = WatchMemory(process, maxMemoryUsageInBytes);
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEndAsync();
        var exitTask = process.WaitForExitAsync();
        
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(timeLimit));
        await Task.WhenAny(exitTask, output, error,memoryUsageTask, Task.Delay(-1, cancellationTokenSource.Token));
        if (cancellationTokenSource.Token.IsCancellationRequested)
        {
            process.Kill();
            result.Status = SubmissionStatus.TimeLimitExceeded;
            result.IsSuccess = false;
            result.ExitCode = 1;
            result.Error = "Time Limit Exceeded";
            return result;
        }

        await exitTask;
        result.ExitCode = process.ExitCode;
        result.IsSuccess = false;
        result.Status = SubmissionStatus.Accepted;
        var executionTimeInMs = (process.ExitTime - process.StartTime).TotalMilliseconds;
        // if (await memoryUsageTask > memoryLimit)
        // {
        //     result.Status = SubmissionStatus.MemoryLimitExceeded;
        // }
        // else
        if (result.ExitCode != 0)
        {
            result.Status = SubmissionStatus.RuntimeError;
            result.Error = await error;
        }
        else if (await output != expectedOutput)
        {
            result.Status = SubmissionStatus.WrongAnswer;
        }
        else
        {
            result.IsSuccess = true;
        }

        result.MemoryUsage = 0;


        result.TimeElapsed = (int)executionTimeInMs;
        return result;
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
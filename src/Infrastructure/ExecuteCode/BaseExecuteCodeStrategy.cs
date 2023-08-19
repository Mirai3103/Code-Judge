using System.Diagnostics;
using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Application.Common.Models;
using Code_Judge.Domain.Enums;

namespace Code_Judge.Infrastructure.ExecuteCode;

public abstract class BaseExecuteCodeStrategy:IExecuteCodeStrategy
{
    private IExecuteCodeStrategy _executeCodeStrategyImplementation;

    protected async Task<float> WatchMemory(Process process, long maxMemoryUsageInBytes)
    {
        var memoryUsageInBytes = 0L;
        while (!process.HasExited)
        {
            if (memoryUsageInBytes < process.PeakVirtualMemorySize64)
            {
                memoryUsageInBytes = process.PeakVirtualMemorySize64;
            }
            if (memoryUsageInBytes > maxMemoryUsageInBytes)
            {
                process.Kill();
                break;
            }
            Console.WriteLine(memoryUsageInBytes);
            await Task.Delay(100);
        }

        return memoryUsageInBytes * 1.0f / (1024 * 1024);
    }

    public async Task<ExecuteCodeResult> WatchProcess(Process process, string input, string expectedOutput,
        int timeLimit, float memoryLimit, CancellationToken cancellationToken = default)
    {
        var result = new ExecuteCodeResult();
        var maxMemoryUsageInBytes = (long)(memoryLimit * 1024 * 1024);
        process.Start();
        await Task.Delay(10);
        await process.StandardInput.WriteLineAsync(input);
        // var memoryUsageTask = WatchMemory(process, maxMemoryUsageInBytes);
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEndAsync();
        var exitTask = process.WaitForExitAsync();
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(timeLimit));
        await Task.WhenAny(exitTask, output, error, Task.Delay(-1, cancellationTokenSource.Token));
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
        else if ((await output).Trim() != expectedOutput.Trim())
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

    public abstract Task<ExecuteCodeResult> ExecuteAsync(string fileName, string input, string expectedOutput, int timeLimit, float memoryLimit, CancellationToken cancellationToken = default);
    public abstract Task<CompileResult> CompileCodeAsync(string code, CancellationToken cancellationToken = default);
}
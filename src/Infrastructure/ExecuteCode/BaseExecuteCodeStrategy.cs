using System.Diagnostics;
using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Application.Common.Models;

namespace Code_Judge.Infrastructure.ExecuteCode;

public abstract class BaseExecuteCodeStrategy:IExecuteCodeStrategy
{
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


    public abstract Task<ExecuteCodeResult> ExecuteCodeAsync(string code, string input, string expectedOutput, int timeLimit, float memoryLimit);
}
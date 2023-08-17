using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Domain.Enums;
using Code_Judge.Infrastructure.ExecuteCode;
using NUnit.Framework;

namespace Code_Judge.Application.IntegrationTests.ExecuteCode;

using static Testing;

public class ExecuteCPPTest
{
    private readonly IExecuteCodeStrategy _executeCppStrategy = GetService<ExecuteCppStrategy>();


    [Test]
    public async Task ShouldSucceed()
    {
        var code = @"
                    #include <iostream>

                    int main() {
                        std::cout << ""Hello World!"";
                        return 0;
                    }";
        var input = @"1 2";
        var expectedOutput = @"Hello World!";
        var timeLimit = 1000;
        var memoryLimit = 20;
        var result = await _executeCppStrategy.ExecuteCodeAsync(code, input, expectedOutput, timeLimit, memoryLimit);
        Assert.AreEqual(SubmissionStatus.Accepted, result.Status);
    }

    [Test]
    public async Task ShouldCompileError()
    {
        var code = @"
                    #include <iostream>

                    int main() {
                        std::cout << ""Hello World!""
                        return 0
                    }";
        var input = @"1 2";
        var expectedOutput = @"Hello World!";
        var timeLimit = 1000;
        var memoryLimit = 20;
        var result = await _executeCppStrategy.ExecuteCodeAsync(code, input, expectedOutput, timeLimit, memoryLimit);
        Assert.AreEqual(SubmissionStatus.CompileError, result.Status);
    }
    [Test]
    public async Task ShouldTimeLimitExceeded()
    {
        var code = @"
                    #include <iostream>
                    int main() {
                        while(true);
                        return 0;
                    }";
        var input = @"1 2";
        var expectedOutput = @"Hello World!";
        var timeLimit = 1000;
        var memoryLimit = 20;
        var result = await _executeCppStrategy.ExecuteCodeAsync(code, input, expectedOutput, timeLimit, memoryLimit);
        Assert.AreEqual(SubmissionStatus.TimeLimitExceeded, result.Status);
    }
    [Test]
    public async Task ShouldMemoryLimitExceeded()
    {
        var code = @"
                    #include <iostream>
                    #include <windows.h>
                    #include <psapi.h>
                    const int N = 100000000;

                    int a[N];
                    int main() {

                    for (int i = 0; i < N; ++i) {
                        a[i] = i;
                     }
                     PROCESS_MEMORY_COUNTERS memCounter;
BOOL result = GetProcessMemoryInfo(GetCurrentProcess(),
                                   &memCounter,
                                   sizeof( memCounter ));
                        std::cout << memCounter.WorkingSetSize << ""\n"";
                        std::cout << memCounter.PeakWorkingSetSize;
                     return 0;
                    }";
        var input = @"1 2";
        var expectedOutput = @"Hello World!";
        var timeLimit = 1000;
        var memoryLimitInMB = 2f;
        var result = await _executeCppStrategy.ExecuteCodeAsync(code, input, expectedOutput, timeLimit, memoryLimitInMB);
        Assert.AreEqual(SubmissionStatus.MemoryLimitExceeded, result.Status);
    }
}
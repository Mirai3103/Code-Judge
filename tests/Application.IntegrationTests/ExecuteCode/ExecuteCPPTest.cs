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
                        while(true){}
                        std::cout << ""Hello World!"";
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
                    const int N = 100000000;

                    int a[N];
                    int main() {

                    for (int i = 0; i < N; ++i) {
                        a[i] = i;
                     }
                    }";
        var input = @"1 2";
        var expectedOutput = @"Hello World!";
        var timeLimit = 1000;
        var memoryLimitInMB = 2f;
        var result = await _executeCppStrategy.ExecuteCodeAsync(code, input, expectedOutput, timeLimit, memoryLimitInMB);
        Assert.AreEqual(SubmissionStatus.MemoryLimitExceeded, result.Status);
    }
    [Test]
    public async Task ShouldVailableToReadInput()
    {
        var code = @"
                     #include <iostream>
                    int main() {
                        int a, b;
                        std::cin >> a >> b;
                        std::cout << a + b;
                        return 0;
                    }";
        var input = @"1 2";
        var expectedOutput = @"3";
        var timeLimit = 1000;
        var memoryLimit = 20;
        var result = await _executeCppStrategy.ExecuteCodeAsync(code, input, expectedOutput, timeLimit, memoryLimit);
        Assert.AreEqual(SubmissionStatus.Accepted, result.Status);
    }
    [Test]
    public async Task ShouldReadListOfString()
    {
        var code = @"
                     #include <iostream>
                     #include <vector>
                     #include <string>
                    int main() {
                        int n;
                        std::cin >> n;
                        std::vector<std::string> a(n);
                        std::cin.ignore();
                        for (int i = 0; i < n; ++i) {
                            std::getline(std::cin, a[i]);
                        }
                        for (int i = 0; i < n; ++i) {
                            std::cout << a[i] << "" "";
                        }
                        return 0;
                    }";
        var input = "2\r\nhello world\r\ni am here";
        var expectedOutput = @"hello world i am here ";
        var timeLimit = 1000;
        var memoryLimit = 20;
        var result = await _executeCppStrategy.ExecuteCodeAsync(code, input, expectedOutput, timeLimit, memoryLimit);
        Assert.AreEqual(SubmissionStatus.Accepted, result.Status);
    }
    [Test]
    public async Task ShouldReadListOfInt()
    {
        var code = @"
                     #include <iostream>
                     #include <vector>
                    int main() {
                        int n;
                        std::cin >> n;
                        std::vector<int> a(n);
                        for (int i = 0; i < n; ++i) {
                            std::cin >> a[i];
                        }
                        for (int i = 0; i < n; ++i) {
                            std::cout << a[i] << "" "";
                        }
                        return 0;
                    }";
        var input = "2\r\n1 2";
        var expectedOutput = @"1 2 ";
        var timeLimit = 1000;
        var memoryLimit = 20;
        var result = await _executeCppStrategy.ExecuteCodeAsync(code, input, expectedOutput, timeLimit, memoryLimit);
        Assert.AreEqual(SubmissionStatus.Accepted, result.Status);
    }
}
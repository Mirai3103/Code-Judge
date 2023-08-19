using Code_Judge.Application.Common.Models;

namespace Code_Judge.Application.Common.Interfaces;

public interface IExecuteCodeStrategy
{
     Task<ExecuteCodeResult> ExecuteAsync(string fileName, string input, string expectedOutput,int timeLimit,float memoryLimit,CancellationToken cancellationToken=default);
        Task<CompileResult> CompileCodeAsync(string code,CancellationToken cancellationToken=default); 
}
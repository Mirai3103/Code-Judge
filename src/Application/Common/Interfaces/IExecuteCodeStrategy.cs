using Code_Judge.Application.Common.Models;

namespace Code_Judge.Application.Common.Interfaces;

public interface IExecuteCodeStrategy
{
     Task<ExecuteCodeResult> ExecuteCodeAsync(string code, string input, string expectedOutput,int timeLimit,float memoryLimit);
}
using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Code_Judge.Infrastructure.ExecuteCode;

public class ExecuteCodeFactory: IExecuteCodeFactory
{
    private readonly IServiceProvider _serviceProvider;
    public ExecuteCodeFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public IExecuteCodeStrategy GetExecuteCodeStrategy(ProgramingLanguage programingLanguage)
    {
        return programingLanguage switch
        {
            ProgramingLanguage.Cpp => _serviceProvider.GetRequiredService<ExecuteCppStrategy>(),
            _ => throw new ArgumentOutOfRangeException(nameof(programingLanguage), programingLanguage, null)
        };
    }
}
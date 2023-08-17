using Code_Judge.Domain.Enums;

namespace Code_Judge.Application.Common.Interfaces;

public interface IExecuteCodeStrategyFactory
{
    IExecuteCodeStrategy GetExecuteCodeStrategy(ProgramingLanguage programingLanguage);
}
using Code_Judge.Domain.Enums;

namespace Code_Judge.Application.Common.Interfaces;

public interface IExecuteCodeFactory
{
    IExecuteCodeStrategy GetExecuteCodeStrategy(ProgramingLanguage programingLanguage);
}
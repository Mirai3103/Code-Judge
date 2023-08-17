namespace Code_Judge.Domain.Enums;

public enum SubmissionStatus
{
    Accepted = 0,
    WrongAnswer = 1,
    MemoryLimitExceeded = 2,
    OutputLimitExceeded = 3,
    TimeLimitExceeded = 4,
    RuntimeError = 5,
    InternalError = 6,
    CompileError = 7,
 
}
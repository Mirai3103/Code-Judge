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
    Rejected = 8,
    Running = 9,
 
}

public static class SubmissionStatusExtension
{
    public static string ToFriendlyString(this SubmissionStatus me)
    {
        switch (me)
        {
            case SubmissionStatus.Accepted:
                return "Accepted";
            case SubmissionStatus.WrongAnswer:
                return "Wrong Answer";
            case SubmissionStatus.MemoryLimitExceeded:
                return "Memory Limit Exceeded";
            case SubmissionStatus.OutputLimitExceeded:
                return "Output Limit Exceeded";
            case SubmissionStatus.TimeLimitExceeded:
                return "Time Limit Exceeded";
            case SubmissionStatus.RuntimeError:
                return "Runtime Error";
            case SubmissionStatus.InternalError:
                return "Internal Error";
            case SubmissionStatus.CompileError:
                return "Compile Error";
            case SubmissionStatus.Rejected:
                return "Rejected";
            case SubmissionStatus.Running:
                return "Running";
            default:
                return "Unknown";
        }
    }
}
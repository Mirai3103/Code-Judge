namespace Code_Judge.Application.Common.Models;

public class Result
{
    internal Result(bool succeeded, ICollection<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    public bool Succeeded { get; init; }

    public string[] Errors { get; init; }

    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }

    public static Result Failure(ICollection<string> errors)
    {
        return new Result(false, errors);
    }
}

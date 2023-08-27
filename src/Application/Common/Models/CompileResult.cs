namespace Code_Judge.Application.Common.Models;

public record CompileResult
{
    public string FileName { get; } = Guid.NewGuid().ToString();
    public bool IsSuccess { get; set; }
    public string? Error { get; set; }
}
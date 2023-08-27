using Code_Judge.Domain.Entities;
using Code_Judge.Domain.Enums;

namespace Code_Judge.Application.Common.Models;

public class ExecuteCodeResult
{
    public string? Error { get; set; }
    public int ExitCode { get; set; }
    public int TimeElapsed { get; set; }
    public float MemoryUsage { get; set; }
    public bool IsSuccess { get; set; }
    public SubmissionStatus Status { get; set; }
    public TestCase? TestCase { get; set; }
    public string? Output { get; set; }
}

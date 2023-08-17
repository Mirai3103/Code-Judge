namespace Code_Judge.Domain.Entities;

public class Submission : BaseAuditableEntity
{
    public string Code { get; set; } = null!;
    public ProgramingLanguage Language { get; set; }
    public string? Error { get; set; }
    public int ProblemId { get; set; }
    public Problem Problem { get; set; } = null!;
    public int RunTime { get; set; } // in ms
    public float Memory { get; set; } // in MB
    public string? Note { get; set; }
    public SubmissionStatus Status { get; set; }
    
}
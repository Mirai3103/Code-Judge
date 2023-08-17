namespace Code_Judge.Domain.Entities;

public class Editorial:BaseAuditableEntity
{
    public int ProblemId { get; set; }
    public Problem Problem { get; set; } = null!;
    public string Content { get; set; } = null!;
}
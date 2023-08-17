namespace Code_Judge.Domain.Entities;

public class TestCase:BaseEntity
{
    public string Input { get; set; } = null!;
    public string Output { get; set; } = null!;
    public int ProblemId { get; set; }
    public Problem Problem { get; set; } = null!;
    public bool IsHidden { get; set; } 
}
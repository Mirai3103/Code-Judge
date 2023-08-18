namespace Code_Judge.Domain.Entities;



public class Problem:BaseAuditableEntity
{
    public int? ContestId { get; set; }
    public Contest? Contest { get; set; }
    public bool IsPublic { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public int Points { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public string? Hint { get; set; }
    public virtual IEnumerable<TestCase> TestCases { get; set; } = new HashSet<TestCase>();
    public virtual IEnumerable<Submission> Submissions { get; set; } = new HashSet<Submission>();
    public Editorial? Editorial { get; set; }
    public int? EditorialId { get; set; }
    public int TimeLimit { get; set; } // in milliseconds
    public float MemoryLimit { get; set; } // in MB
}
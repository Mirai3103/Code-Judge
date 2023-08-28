namespace Code_Judge.Domain.Entities;

public class Contest : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public virtual ICollection<Problem> Problems { get; set; } = new HashSet<Problem>();
    public string? Description { get; set; }
}
using Code_Judge.Application.Common.Mappings;
using Code_Judge.Domain.Entities;
using Code_Judge.Domain.Enums;

namespace Code_Judge.Application.Problems.Queries.GetProblemsWithPagination;

public class ProblemBriefDto:IMapFrom<Problem>
{
    public int? ContestId { get; set; }
    public bool IsPublic { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public int Points { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
}
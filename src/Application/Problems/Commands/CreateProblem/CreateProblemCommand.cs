using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Application.Common.Slug;
using Code_Judge.Domain.Entities;
using Code_Judge.Domain.Enums;
using MediatR;

namespace Code_Judge.Application.Problems.Commands.CreateProblem;

public record CreateProblemCommand : IRequest<int>
{
    public int? ContestId { get; set; }
    public bool IsPublic { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Points { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public string TemplateCode { get; set; } = null!;
    public string? Hint { get; set; }
    public int TimeLimit { get; set; } // in milliseconds
    public float MemoryLimit { get; set; } // in MB
}

public class CreateProblemCommandHandler : IRequestHandler<CreateProblemCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateProblemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateProblemCommand request, CancellationToken cancellationToken)
    {
        var newEntity = new Problem()
        {
            Slug = request.Name.Slugify(),
            Hint = request.Hint,
            IsPublic = request.IsPublic,
            Name = request.Name,
            Description = request.Description,
            Points = request.Points,
            DifficultyLevel = request.DifficultyLevel,
            TemplateCode = request.TemplateCode,
            TimeLimit = request.TimeLimit,
            MemoryLimit = request.MemoryLimit,
            ContestId = request.ContestId
        };
        _context.Problems.Add(newEntity);
        await _context.SaveChangesAsync(cancellationToken);
        return newEntity.Id;
    }
}
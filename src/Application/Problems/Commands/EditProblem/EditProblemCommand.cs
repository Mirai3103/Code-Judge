using Code_Judge.Application.Common.Exceptions;
using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Application.Common.Slug;
using Code_Judge.Domain.Entities;
using Code_Judge.Domain.Enums;
using MediatR;

namespace Code_Judge.Application.Problems.Commands.EditProblem;

public record EditProblemCommand(int Id) :IRequest<int>
{
    public int? ContestId { get; set; }
    public bool IsPublic { get; set; } = true;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Points { get; set; } =1;
    public DifficultyLevel DifficultyLevel { get; set; }
    public string? Hint { get; set; }
    public int TimeLimit { get; set; } // in milliseconds
    public float MemoryLimit { get; set; } // in MB
}


public class EditProblemCommandHandler : IRequestHandler<EditProblemCommand, int>
{
    private readonly IApplicationDbContext _context;

    public EditProblemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(EditProblemCommand request, CancellationToken cancellationToken)
    {
        var oldEntity = await _context.Problems.FindAsync(request.Id);
        if (oldEntity == null)
        {
            throw new NotFoundException(nameof(Problem), request.Name);
        }
        oldEntity.Slug = request.Name.Slugify();
        oldEntity.Hint = request.Hint;
        oldEntity.IsPublic = request.IsPublic;
        oldEntity.Name = request.Name;
        oldEntity.Description = request.Description;
        oldEntity.Points = request.Points;
        oldEntity.DifficultyLevel = request.DifficultyLevel;
        oldEntity.TimeLimit = request.TimeLimit;
        oldEntity.MemoryLimit = request.MemoryLimit;
        oldEntity.ContestId = request.ContestId;
        await _context.SaveChangesAsync(cancellationToken);
        return oldEntity.Id;
    }
}
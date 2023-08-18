using Code_Judge.Application.Common.Exceptions;
using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Domain.Entities;
using MediatR;

namespace Code_Judge.Application.Problems.Commands.DeleteProblem;

public record DeleteProblemCommand(int Id) : IRequest;

public class DeleteProblemCommandHandler : IRequestHandler<DeleteProblemCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteProblemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task Handle(DeleteProblemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Problems.FindAsync(new object[]
        {
            request.Id
        }, cancellationToken);
        if (entity is null)
        {
            throw new NotFoundException(nameof(Problem), request.Id);
        }
        _context.Problems.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
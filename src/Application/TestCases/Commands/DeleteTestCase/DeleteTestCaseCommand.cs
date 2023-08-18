using Code_Judge.Application.Common.Exceptions;
using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Domain.Entities;
using MediatR;

namespace Code_Judge.Application.TestCases.Commands.DeleteTestCase;

public record DeleteTestCaseCommand(int Id) : IRequest;
public class DeleteTestCaseCommandHandler : IRequestHandler<DeleteTestCaseCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteTestCaseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task Handle(DeleteTestCaseCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TestCases.FindAsync(new object[]
        {
            request.Id
        }, cancellationToken);
        if (entity is null)
        {
            throw new NotFoundException(nameof(TestCase), request.Id);
        }
        _context.TestCases.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}


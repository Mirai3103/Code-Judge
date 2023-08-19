using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Domain.Entities;
using MediatR;

namespace Code_Judge.Application.Problems.Queries.GetProblemById;

public record GetProblemByIdQuery(int Id) : IRequest<Problem?>;

public class GetProblemByIdQueryHandler : IRequestHandler<GetProblemByIdQuery, Problem?>
{
    private readonly IApplicationDbContext _context;

    public GetProblemByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Problem?> Handle(GetProblemByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Problems.FindAsync(request.Id, cancellationToken);
    }
}
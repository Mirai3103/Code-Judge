using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Domain.Entities;
using Microsoft.EntityFrameworkCore;


using MediatR;

namespace Code_Judge.Application.Problems.Queries.GetProblemBySlug;

public record GetProblemBySlugQuery(string slug) : IRequest<Problem?>;

public class GetProblemByIdQueryHandler : IRequestHandler<GetProblemBySlugQuery, Problem?>
{
    private readonly IApplicationDbContext _context;

    public GetProblemByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Problem?> Handle(GetProblemBySlugQuery request, CancellationToken cancellationToken)
    {
        return await _context.Problems.FirstOrDefaultAsync(p => p.Slug == request.slug, cancellationToken);
    }
}
using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Code_Judge.Application.TestCases.Queries.GetTestCases;

public record GetTestCasesQuery (int ProblemId) : IRequest<IEnumerable<TestCase>>;
public class GetTestCasesQueryHandler : IRequestHandler<GetTestCasesQuery, IEnumerable<TestCase>>
{
    private readonly IApplicationDbContext _context;
    public GetTestCasesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<TestCase>> Handle(GetTestCasesQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.TestCases.Where(x => x.ProblemId == request.ProblemId).ToListAsync(cancellationToken);
        return entity;
    }
}
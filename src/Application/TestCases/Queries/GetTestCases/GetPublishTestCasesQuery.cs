using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Code_Judge.Application.TestCases.Queries.GetTestCases;

public record GetPublishTestCasesQuery(int ProblemId) : IRequest<IEnumerable<TestCase>>;

public class GetPublishTestCasesQueryHandler : IRequestHandler<GetPublishTestCasesQuery, IEnumerable<TestCase>>
{
    private readonly IApplicationDbContext _context;

    public GetPublishTestCasesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TestCase>> Handle(GetPublishTestCasesQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.TestCases.Where(x => x.ProblemId == request.ProblemId && x.IsHidden == false).ToListAsync(cancellationToken);
        return entity;
    }
}
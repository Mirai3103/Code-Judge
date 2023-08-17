using AutoMapper;
using AutoMapper.QueryableExtensions;
using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Application.Common.Mappings;
using Code_Judge.Application.Common.Models;
using MediatR;

namespace Code_Judge.Application.Problems.Queries.GetProblemsWithPagination;

public record GetProblemsWithPaginationQuery : IRequest<PaginatedList<ProblemBriefDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public class GetProblemsWithPaginationQueryHandler : IRequestHandler<GetProblemsWithPaginationQuery,PaginatedList<ProblemBriefDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProblemsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ProblemBriefDto>> Handle(GetProblemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.Problems
            .Where(x =>x.IsPublic)
            .OrderByDescending(p=>p.Created)
            .ProjectTo<ProblemBriefDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}

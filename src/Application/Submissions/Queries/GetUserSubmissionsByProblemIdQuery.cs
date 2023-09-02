using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Code_Judge.Application.Submissions.Queries;

public record GetUserSubmissionsByProblemIdQuery(int Id) : IRequest<ICollection<Submission>>;



public class GetUserSubmissionsByProblemIdQueryQueryHandler : IRequestHandler<GetUserSubmissionsByProblemIdQuery, ICollection<Submission>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    public GetUserSubmissionsByProblemIdQueryQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }
    public async Task<ICollection<Submission>> Handle(GetUserSubmissionsByProblemIdQuery request, CancellationToken cancellationToken)
    {
        if(_currentUserService.UserId is null)
            throw new UnauthorizedAccessException();
        var submissions = await _context.Submissions
            .Where(s => s.CreatedBy == _currentUserService.UserId && s.ProblemId == request.Id)
            .OrderByDescending(s => s.Created)
            .ToListAsync(cancellationToken: cancellationToken);
        return submissions;
    }
}
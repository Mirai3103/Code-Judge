using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Code_Judge.Application.Submissions.Queries;

public record GetUserLatestSubmissionQuery:IRequest<Submission?>
{
    
}

public class GetUserLatestSubmissionQueryHandler:IRequestHandler<GetUserLatestSubmissionQuery,Submission?>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetUserLatestSubmissionQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }
    public async Task<Submission?> Handle(GetUserLatestSubmissionQuery request, CancellationToken cancellationToken)
    {
        if(_currentUserService.UserId is null)
            throw new UnauthorizedAccessException();
        var submission = await _context.Submissions
            .Where(s => s.CreatedBy == _currentUserService.UserId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        return submission;
    }
}
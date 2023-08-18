using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Domain.Entities;
using MediatR;

namespace Code_Judge.Application.TestCases.Commands.CreateTestCase;

public class CreateTestCaseCommand : IRequest<int>
{
    public string Input { get; set; } = null!;
    public string Output { get; set; } = null!;
    public int ProblemId { get; set; }
    public bool IsHidden { get; set; }
}

public class CreateTestCaseCommandHandler : IRequestHandler<CreateTestCaseCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateTestCaseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTestCaseCommand request, CancellationToken cancellationToken)
    {
        var entity = new TestCase()
        {
            Input = request.Input, Output = request.Output, ProblemId = request.ProblemId, IsHidden = request.IsHidden,
        };
        await _context.TestCases.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
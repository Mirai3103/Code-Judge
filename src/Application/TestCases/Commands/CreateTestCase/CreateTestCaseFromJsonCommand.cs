using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Code_Judge.Application.TestCases.Commands.CreateTestCase;

public record CreateTestCaseFromJsonCommand: IRequest<IEnumerable<int>>
{
    public string Json { get; set; } = null!;
}

public class CreateTestCaseFromJsonCommandHandler : IRequestHandler<CreateTestCaseFromJsonCommand, IEnumerable<int>>
{
    private readonly IMediator _mediator;

    public CreateTestCaseFromJsonCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IEnumerable<int>> Handle(CreateTestCaseFromJsonCommand request, CancellationToken cancellationToken)
    {
        var command = JsonSerializer.Deserialize<CreateTestCaseCommand[]>(request.Json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if (command is null)
        {
            throw new ValidationException(new ValidationFailure[] {new(nameof(request.Json), "Json is invalid")});
        }
        var ids = new List<int>();
        foreach (var createTestCaseCommand in command)
        {
            ids.Add(await _mediator.Send(createTestCaseCommand, cancellationToken));
        }
        return ids;
    }
}
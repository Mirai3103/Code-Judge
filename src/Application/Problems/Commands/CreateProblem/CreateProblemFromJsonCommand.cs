using System.Text.Json;
using Code_Judge.Application.Common.Exceptions;
using FluentValidation.Results;
using MediatR;

namespace Code_Judge.Application.Problems.Commands.CreateProblem;

public record CreateProblemFromJsonCommand(string Json) : IRequest<IEnumerable<int>>;

public class CreateProblemFromJsonCommandHandler : IRequestHandler<CreateProblemFromJsonCommand, IEnumerable<int>>
{
    private readonly IMediator _mediator;
    
    public CreateProblemFromJsonCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task< IEnumerable<int>> Handle(CreateProblemFromJsonCommand request, CancellationToken cancellationToken)
    {
        var command = JsonSerializer.Deserialize<CreateProblemCommand[]>(request.Json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if (command is null)
        {
            throw new ValidationException(new ValidationFailure[] {new(nameof(request.Json), "Json is invalid")});
        }
        var result = new List<int>();
        foreach (var createProblemCommand in command)
        {
            result.Add(await _mediator.Send(createProblemCommand, cancellationToken));
        }
        return result;
    }
}
using FluentValidation;

namespace Code_Judge.Application.Problems.Commands.CreateProblem;

public class CreateProblemCommandValidator : AbstractValidator<CreateProblemCommand>
{
    public CreateProblemCommandValidator()
    {
        RuleFor(v => v.Description)
            .MinimumLength(50)
            .NotEmpty();
        RuleFor(v => v.Name)
            .MaximumLength(10)
            .MaximumLength(200)
            .NotEmpty();
        RuleFor(v => v.MemoryLimit).GreaterThan(0)
            .NotNull();
        RuleFor(v => v.TimeLimit).GreaterThan(0).NotNull();
        RuleFor(v => v.Points).GreaterThan(0).NotNull();
        RuleFor(v => v.Hint).MaximumLength(200);
        RuleFor(v => v.DifficultyLevel).NotNull();
    }
}
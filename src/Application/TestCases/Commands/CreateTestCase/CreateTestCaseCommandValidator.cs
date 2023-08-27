using FluentValidation;

namespace Code_Judge.Application.TestCases.Commands.CreateTestCase;

public class CreateTestCaseCommandValidator: AbstractValidator<CreateTestCaseCommand>
{
    public CreateTestCaseCommandValidator()
    {
        RuleFor(v => v.Input)
            .NotEmpty().WithMessage("Input is required.");

        RuleFor(v => v.Output)
            .NotEmpty().WithMessage("Output is required.");
        RuleFor(v => v.ProblemId)
            .NotEqual(0).WithMessage("ProblemId is required.");
    }
}
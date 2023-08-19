using FluentValidation;

namespace Code_Judge.Application.TestCases.Commands.CreateTestCase;

public class CreateTestCaseCommandValidator: AbstractValidator<CreateTestCaseCommand>
{
    public CreateTestCaseCommandValidator()
    {
        RuleFor(v => v.Input)
            .NotEmpty().WithMessage("Input is required.");
          
        RuleFor(v => v.Output)
            .NotEmpty().WithMessage("Output is required.")
            .MaximumLength(200).WithMessage("Output must not exceed 200 characters.");
        RuleFor(v => v.ProblemId)
            .NotEmpty().WithMessage("ProblemId is required.");
    }
}
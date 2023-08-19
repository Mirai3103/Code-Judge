using Code_Judge.Domain.Enums;
using FluentValidation;

namespace Code_Judge.Application.Submissions.TestSubmission;

public class TestSubmissionCommandValidator: AbstractValidator<TestSubmissionCommand>
{
    public TestSubmissionCommandValidator()
    {
        RuleFor(v => v.Code)
            .NotEmpty().WithMessage("Code is required.");
        RuleFor(v => v.ProblemId)
            .NotEmpty().WithMessage("ProblemId is required.");
        RuleFor(v => v.Language)
            .NotNull()
            .IsInEnum().WithMessage("Language is not valid.");
    }
}
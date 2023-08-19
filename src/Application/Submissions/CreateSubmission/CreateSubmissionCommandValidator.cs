using Code_Judge.Domain.Enums;
using FluentValidation;

namespace Code_Judge.Application.Submissions.CreateSubmission;

public class CreateSubmissionCommandValidator: AbstractValidator<CreateSubmissionCommand>
{
    public CreateSubmissionCommandValidator()
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

using FluentValidation;

namespace Wakiliy.Application.Features.Lawyers.Commands.Verification.RejectVerification;

public class RejectVerificationCommandValidator : AbstractValidator<RejectVerificationCommand>
{
    public RejectVerificationCommandValidator()
    {
        RuleFor(x => x.LawyerId)
            .NotEmpty();

        RuleFor(x => x.Note)
            .MaximumLength(1000)
            .When(x => x.Note is not null);
    }
}

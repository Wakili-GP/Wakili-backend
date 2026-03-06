using FluentValidation;

namespace Wakiliy.Application.Features.Lawyers.Commands.Verification.ApproveVerification;

public class ApproveVerificationCommandValidator : AbstractValidator<ApproveVerificationCommand>
{
    public ApproveVerificationCommandValidator()
    {
        RuleFor(x => x.LawyerId)
            .NotEmpty();
    }
}

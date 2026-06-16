using FluentValidation;

namespace Wakiliy.Application.Features.Auth.Commands.ConfirmEmail;
public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty();
    }
}

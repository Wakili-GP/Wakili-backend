using FluentValidation;

namespace Wakiliy.Application.Features.Account.Commands.UpdateClientInfo
{
    public class UpdateClientInfoCommandValidator : AbstractValidator<UpdateClientInfoCommand>
    {
        public UpdateClientInfoCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20)
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.FullName)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.FullName));

            RuleFor(x => x.Address)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Address));
        }
    }
}

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

            RuleFor(x => x.FirstName)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.FirstName));

            RuleFor(x => x.LastName)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.LastName));

            RuleFor(x => x.City)
                .MaximumLength(150)
                .When(x => !string.IsNullOrWhiteSpace(x.City));

            RuleFor(x => x.Country)
                .MaximumLength(150)
                .When(x => !string.IsNullOrWhiteSpace(x.Country));
        }
    }
}

using FluentValidation;

namespace Wakiliy.Application.Features.Account.Commands.Update
{
    public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
    {
        public UpdateAccountCommandValidator()
        {
            RuleFor(x => x.FullName)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.FullName));

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20)
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.ImageUrl)
                .MaximumLength(2048)
                .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));

            RuleFor(x => x.Gender)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.Gender));

            RuleFor(x => x.Address)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Address));
        }
    }
}
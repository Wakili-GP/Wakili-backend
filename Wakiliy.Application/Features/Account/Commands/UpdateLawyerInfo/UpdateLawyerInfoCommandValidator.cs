using FluentValidation;

namespace Wakiliy.Application.Features.Account.Commands.UpdateLawyerInfo
{
    public class UpdateLawyerInfoCommandValidator : AbstractValidator<UpdateLawyerInfoCommand>
    {
        public UpdateLawyerInfoCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20)
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.City)
                .MaximumLength(150)
                .When(x => !string.IsNullOrWhiteSpace(x.City));

            RuleFor(x => x.Country)
                .MaximumLength(150)
                .When(x => !string.IsNullOrWhiteSpace(x.Country));

            RuleFor(x => x.Summary)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.Summary));
                
            RuleFor(x => x.Bio)
                .MaximumLength(600)
                .When(x => !string.IsNullOrWhiteSpace(x.Bio));

            RuleFor(x => x.PhoneSessionPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.PhoneSessionPrice.HasValue);

            RuleFor(x => x.InOfficeSessionPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.InOfficeSessionPrice.HasValue);
        }
    }
}

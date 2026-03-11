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

            RuleFor(x => x.FullName)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.FullName));

            RuleFor(x => x.Address)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Address));

            RuleFor(x => x.LicenseNumber)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.LicenseNumber));

            RuleFor(x => x.SpecializationIds)
                .Must(list => list == null || list.All(id => id > 0))
                .WithMessage("Specialization ids must be positive numbers");

            RuleFor(x => x.YearsOfExperience)
                .GreaterThanOrEqualTo(0)
                .When(x => x.YearsOfExperience.HasValue);

            RuleFor(x => x.PhoneSessionPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.PhoneSessionPrice.HasValue);

            RuleFor(x => x.InOfficeSessionPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.InOfficeSessionPrice.HasValue);
        }
    }
}

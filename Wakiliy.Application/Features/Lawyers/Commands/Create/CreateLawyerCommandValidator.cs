using FluentValidation;
using Wakiliy.Application.Features.Lawyers.Commands.Create;

namespace Wakiliy.Application.Features.Lawyers.Commands.Create
{
    public class CreateLawyerCommandValidator : AbstractValidator<CreateLaywerCommand>
    {
        public CreateLawyerCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress();

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(50);


            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Address)
                .MaximumLength(500);

            RuleFor(x => x.LicenseNumber)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.SpecializationIds)
                .NotEmpty();

            RuleFor(x => x.YearsOfExperience)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.VerificationStatus)
                .IsInEnum();
        }
    }
}

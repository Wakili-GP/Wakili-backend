using FluentValidation;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveVerification;

public class SaveVerificationCommandValidator : AbstractValidator<SaveVerificationCommand>
{
    public SaveVerificationCommandValidator()
    {
        RuleFor(x => x.NationalIdFront).NotNull().WithMessage("National ID front image is required");
        RuleFor(x => x.NationalIdBack).NotNull().WithMessage("National ID back image is required");

        RuleFor(x => x.License)
             .NotNull().WithMessage("License information is required.")
             .SetValidator(new OnBoardingLawyerLicenseDtoValidator());


        RuleForEach(x => x.EducationalCertificates)
            .NotNull()
            .WithMessage("Educational certificate files cannot be null");
    }

    public class OnBoardingLawyerLicenseDtoValidator : AbstractValidator<OnBoardingLawyerLicenseDto>
    {
        public OnBoardingLawyerLicenseDtoValidator()
        {
            RuleFor(x => x.LicenseFile)
                .NotNull().WithMessage("Lawyer license image is required.");

            RuleFor(x => x.LicenseNumber)
                .NotEmpty().WithMessage("License number is required.")
                .MaximumLength(100).WithMessage("License number cannot exceed 100 characters.");

            RuleFor(x => x.IssuingAuthority)
                .NotEmpty().WithMessage("Issuing authority is required.")
                .MaximumLength(200).WithMessage("Issuing authority cannot exceed 200 characters.");

            RuleFor(x => x.LicenseYear)
                .NotEmpty().WithMessage("License year is required.")
                .Matches(@"^\d{4}$").WithMessage("License year must be a 4-digit year.");
        }
    }
}

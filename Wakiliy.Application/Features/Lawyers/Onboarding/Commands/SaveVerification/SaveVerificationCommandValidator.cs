using FluentValidation;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveVerification;

public class SaveVerificationCommandValidator : AbstractValidator<SaveVerificationCommand>
{
    public SaveVerificationCommandValidator()
    {
        RuleFor(x => x.NationalIdFront).NotNull().WithMessage("National ID front image is required");
        RuleFor(x => x.NationalIdBack).NotNull().WithMessage("National ID back image is required");
        RuleFor(x => x.LawyerLicense).NotNull().WithMessage("Lawyer license image is required");

        RuleForEach(x => x.EducationalCertificates)
            .NotNull()
            .WithMessage("Educational certificate files cannot be null");

        RuleForEach(x => x.ProfessionalCertificates)
            .NotNull()
            .WithMessage("Professional certificate files cannot be null");
    }

}

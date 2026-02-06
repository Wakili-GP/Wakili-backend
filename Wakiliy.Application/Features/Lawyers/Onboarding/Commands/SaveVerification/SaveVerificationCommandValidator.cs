using FluentValidation;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveVerification;

public class SaveVerificationCommandValidator : AbstractValidator<SaveVerificationCommand>
{
    public SaveVerificationCommandValidator()
    {
        RuleFor(x => x.NationalIdFront).SetValidator(new UploadedDocumentValidator());
        RuleFor(x => x.NationalIdBack).SetValidator(new UploadedDocumentValidator());
        RuleFor(x => x.LawyerLicense).SetValidator(new UploadedDocumentValidator());

        RuleForEach(x => x.EducationalCertificates)
            .SetValidator(new UploadedDocumentValidator());

        RuleForEach(x => x.ProfessionalCertificates)
            .SetValidator(new UploadedDocumentValidator());
    }

    private class UploadedDocumentValidator : AbstractValidator<UploadedDocumentDto>
    {
        public UploadedDocumentValidator()
        {
            RuleFor(x => x.File)
                .NotEmpty();

            RuleFor(x => x.FileName)
                .NotEmpty();

            RuleFor(x => x.Status)
                .NotEmpty();
        }
    }
}

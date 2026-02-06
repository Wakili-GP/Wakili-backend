using FluentValidation;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveEducation;

public class SaveEducationCommandValidator : AbstractValidator<SaveEducationCommand>
{
    public SaveEducationCommandValidator()
    {
        RuleFor(x => x.AcademicQualifications)
            .NotEmpty();

        RuleForEach(x => x.AcademicQualifications)
            .SetValidator(new AcademicQualificationDtoValidator());

        RuleForEach(x => x.ProfessionalCertifications)
            .SetValidator(new ProfessionalCertificationDtoValidator());
    }

    private class AcademicQualificationDtoValidator : AbstractValidator<AcademicQualificationDto>
    {
        public AcademicQualificationDtoValidator()
        {
            RuleFor(x => x.DegreeType).NotEmpty();
            RuleFor(x => x.FieldOfStudy).NotEmpty();
            RuleFor(x => x.UniversityName).NotEmpty();
            RuleFor(x => x.GraduationYear)
                .NotEmpty()
                .Matches("^\\d{4}$");
        }
    }

    private class ProfessionalCertificationDtoValidator : AbstractValidator<ProfessionalCertificationDto>
    {
        public ProfessionalCertificationDtoValidator()
        {
            RuleFor(x => x.CertificateName).NotEmpty();
            RuleFor(x => x.IssuingOrganization).NotEmpty();
            RuleFor(x => x.YearObtained)
                .NotEmpty()
                .Matches("^\\d{4}$");
            RuleFor(x => x.Document).NotEmpty();
        }
    }
}

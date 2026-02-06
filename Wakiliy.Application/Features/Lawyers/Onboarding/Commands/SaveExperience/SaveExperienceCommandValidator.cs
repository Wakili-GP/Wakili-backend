using FluentValidation;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveExperience;

public class SaveExperienceCommandValidator : AbstractValidator<SaveExperienceCommand>
{
    public SaveExperienceCommandValidator()
    {
        RuleFor(x => x.WorkExperiences)
            .NotEmpty();

        RuleForEach(x => x.WorkExperiences)
            .SetValidator(new WorkExperienceValidator());
    }

    private class WorkExperienceValidator : AbstractValidator<WorkExperienceDto>
    {
        public WorkExperienceValidator()
        {
            RuleFor(x => x.JobTitle).NotEmpty();
            RuleFor(x => x.OrganizationName).NotEmpty();
            RuleFor(x => x.StartYear)
                .NotEmpty()
                .Matches("^\\d{4}$");

            RuleFor(x => x)
                .Must(HasValidYearRange)
                .When(x => !string.IsNullOrWhiteSpace(x.EndYear))
                .WithMessage("End year must be greater than or equal to start year");
        }

        private static bool HasValidYearRange(WorkExperienceDto dto)
        {
            return int.TryParse(dto.StartYear, out var start)
                   && int.TryParse(dto.EndYear, out var end)
                   && end >= start;
        }
    }
}

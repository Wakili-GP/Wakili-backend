using FluentValidation;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveBasicInfo;

public class SaveBasicInfoCommandValidator : AbstractValidator<SaveBasicInfoCommand>
{
    public SaveBasicInfoCommandValidator()
    {
        RuleFor(x => x.Country)
            .NotEmpty();

        RuleFor(x => x.City)
            .NotEmpty();

        RuleFor(x => x.PracticeAreas)
            .NotEmpty();

        RuleFor(x => x.SessionTypes)
            .NotEmpty()
            .Must(list => list.All(value => !string.IsNullOrWhiteSpace(value)))
            .WithMessage("Session types cannot contain empty values");
    }
}

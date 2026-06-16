using FluentValidation;

namespace Wakiliy.Application.Features.Lawyers.Commands.RemoveFavorite;

public class RemoveFavoriteLawyerCommandValidator : AbstractValidator<RemoveFavoriteLawyerCommand>
{
    public RemoveFavoriteLawyerCommandValidator()
    {
        RuleFor(x => x.LawyerId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}

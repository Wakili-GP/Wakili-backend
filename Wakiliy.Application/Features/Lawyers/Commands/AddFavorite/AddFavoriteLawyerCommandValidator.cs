using FluentValidation;

namespace Wakiliy.Application.Features.Lawyers.Commands.AddFavorite;

public class AddFavoriteLawyerCommandValidator : AbstractValidator<AddFavoriteLawyerCommand>
{
    public AddFavoriteLawyerCommandValidator()
    {
        RuleFor(x => x.LawyerId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}

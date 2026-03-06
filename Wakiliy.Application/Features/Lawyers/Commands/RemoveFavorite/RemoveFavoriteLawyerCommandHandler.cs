using MediatR;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.RemoveFavorite;

public class RemoveFavoriteLawyerCommandHandler(
    IFavoriteLawyerRepository favoriteLawyerRepository) : IRequestHandler<RemoveFavoriteLawyerCommand, Result>
{
    public async Task<Result> Handle(RemoveFavoriteLawyerCommand request, CancellationToken cancellationToken)
    {
        var exists = await favoriteLawyerRepository.ExistsAsync(request.UserId, request.LawyerId, cancellationToken);
        if (!exists)
            return Result.Failure(FavoriteErrors.NotInFavorites);

        await favoriteLawyerRepository.RemoveAsync(request.UserId, request.LawyerId, cancellationToken);
        return Result.Success();
    }
}

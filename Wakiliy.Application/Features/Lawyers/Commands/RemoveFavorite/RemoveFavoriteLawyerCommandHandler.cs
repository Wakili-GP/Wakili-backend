using MediatR;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.RemoveFavorite;

public class RemoveFavoriteLawyerCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<RemoveFavoriteLawyerCommand, Result>
{
    public async Task<Result> Handle(RemoveFavoriteLawyerCommand request, CancellationToken cancellationToken)
    {
        var exists = await unitOfWork.FavoriteLawyers.ExistsAsync(request.UserId, request.LawyerId, cancellationToken);
        if (!exists)
            return Result.Failure(FavoriteErrors.NotInFavorites);

        await unitOfWork.FavoriteLawyers.RemoveAsync(request.UserId, request.LawyerId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

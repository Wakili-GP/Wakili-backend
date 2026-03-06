using MediatR;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.AddFavorite;

public class AddFavoriteLawyerCommandHandler(
    IFavoriteLawyerRepository favoriteLawyerRepository,
    ILawyerRepository lawyerRepository) : IRequestHandler<AddFavoriteLawyerCommand, Result>
{
    public async Task<Result> Handle(AddFavoriteLawyerCommand request, CancellationToken cancellationToken)
    {
        var lawyer = await lawyerRepository.GetByIdAsync(request.LawyerId, cancellationToken);
        if (lawyer is null)
            return Result.Failure(FavoriteErrors.LawyerNotFound);

        var alreadyFavorited = await favoriteLawyerRepository.ExistsAsync(request.UserId, request.LawyerId, cancellationToken);
        if (alreadyFavorited)
            return Result.Failure(FavoriteErrors.AlreadyFavorited);

        var favorite = new FavoriteLawyer
        {
            UserId = request.UserId,
            LawyerId = request.LawyerId
        };

        await favoriteLawyerRepository.AddAsync(favorite, cancellationToken);
        return Result.Success();
    }
}

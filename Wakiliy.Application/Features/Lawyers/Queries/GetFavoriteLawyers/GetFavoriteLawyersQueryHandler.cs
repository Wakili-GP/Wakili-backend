using Mapster;
using MediatR;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetFavoriteLawyers;

public class GetFavoriteLawyersQueryHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<GetFavoriteLawyersQuery, Result<List<LawyerResponse>>>
{
    public async Task<Result<List<LawyerResponse>>> Handle(GetFavoriteLawyersQuery request, CancellationToken cancellationToken)
    {
        var lawyers = await unitOfWork.FavoriteLawyers.GetFavoriteLawyersAsync(request.UserId, cancellationToken);
        var response = lawyers.Adapt<List<LawyerResponse>>();
        return Result.Success(response);
    }
}

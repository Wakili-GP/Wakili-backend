using MediatR;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetFavoriteLawyers;

public class GetFavoriteLawyersQuery(string userId) : IRequest<Result<List<LawyerResponse>>>
{
    public string UserId { get; } = userId;
}

using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.RemoveFavorite;

public class RemoveFavoriteLawyerCommand : IRequest<Result>
{
    public string UserId { get; set; } = string.Empty;
    public string LawyerId { get; set; } = string.Empty;
}

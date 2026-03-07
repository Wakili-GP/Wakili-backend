using MediatR;
using Wakiliy.Application.Features.Admins.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Admins.Queries.GetUsers
{
    public class GetUsersQuery : IRequest<Result<IEnumerable<UserListItemDto>>>
    {
    }
}

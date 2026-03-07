using Mapster;
using MediatR;
using Wakiliy.Application.Features.Admins.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Admins.Queries.GetUsers
{
    public class GetUsersQueryHandler(IUserRepository userRepository)
        : IRequestHandler<GetUsersQuery, Result<IEnumerable<UserListItemDto>>>
    {
        public async Task<Result<IEnumerable<UserListItemDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await userRepository.GetUsersAsync();
            return Result.Success(users.Adapt<IEnumerable<UserListItemDto>>());
        }
    }
}

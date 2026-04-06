using Mapster;
using MediatR;
using Wakiliy.Application.Common.Models;
using Wakiliy.Application.Features.Users.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Users.Queries.GetUsers
{
    public class GetUsersQueryHandler(IUserRepository userRepository)
        : IRequestHandler<GetUsersQuery, Result<PaginatedResult<UserListItemDto>>>
    {
        public async Task<Result<PaginatedResult<UserListItemDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var (users, totalCount) = await userRepository.GetUsersPagedAsync(
                request.Page,
                request.PageSize,
                request.Name,
                request.UserType,
                request.Status
            );

            var dtoItems = users.Adapt<List<UserListItemDto>>();

            var paginatedResult = new PaginatedResult<UserListItemDto>
            {
                Items = dtoItems,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };

            return Result.Success(paginatedResult);
        }
    }
}

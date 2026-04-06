using MediatR;
using Wakiliy.Application.Common.Models;
using Wakiliy.Application.Features.Users.DTOs;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Users.Queries.GetUsers
{
    public class GetUsersQuery : IRequest<Result<PaginatedResult<UserListItemDto>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Name { get; set; }
        public string? UserType { get; set; }
        public UserStatus? Status { get; set; }
    }
}

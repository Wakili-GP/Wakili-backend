using MediatR;
using System.Text.Json.Serialization;
using Wakiliy.Application.Features.Admins.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Admins.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<Result<UserListItemDto>>
    {
        [JsonIgnore]
        public string Id { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}

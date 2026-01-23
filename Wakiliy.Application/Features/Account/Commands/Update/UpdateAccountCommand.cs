using MediatR;
using System.Text.Json.Serialization;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Commands.Update
{
    public class UpdateAccountCommand : IRequest<Result<UserInfoResponse>>
    {
        [JsonIgnore]
        public string Id { get; set; } = string.Empty;

        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
    }
}
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Commands.UpdateClientInfo
{
    public class UpdateClientInfoDto {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? Bio { get; set; }
    }
    public class UpdateClientInfoCommand : IRequest<Result<UserInfoResponse>>
    {
        [JsonIgnore]
        public string Id { get; set; } = string.Empty;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? Address { get; set; }
        public string? Bio { get; set; }
    }
}

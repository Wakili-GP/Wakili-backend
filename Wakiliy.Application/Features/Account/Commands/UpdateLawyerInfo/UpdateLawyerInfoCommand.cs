using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Commands.UpdateLawyerInfo
{
    public class UpdateLawyerInfoDto {
        public string? PhoneNumber { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Bio { get; set; }
        public string? Summary { get; set; }
        public decimal? PhoneSessionPrice { get; set; }
        public decimal? InOfficeSessionPrice { get; set; }
    }
    public class UpdateLawyerInfoCommand : IRequest<Result<LawyerDataDto>>
    {
        [JsonIgnore]
        public string Id { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Bio { get; set; }
        public string? Summary { get; set; }
        public decimal? PhoneSessionPrice { get; set; }
        public decimal? InOfficeSessionPrice { get; set; }
    }
}

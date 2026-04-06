using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Commands.UpdateLawyerInfo
{
    public class UpdateLawyerInfoDto {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }

        public string? LicenseNumber { get; set; }
        public List<int>? SpecializationIds { get; set; }
        public int? YearsOfExperience { get; set; }
        public decimal? PhoneSessionPrice { get; set; }
        public decimal? InOfficeSessionPrice { get; set; }
    }
    public class UpdateLawyerInfoCommand : IRequest<Result<UserInfoResponse>>
    {
        [JsonIgnore]
        public string Id { get; set; } = string.Empty;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }

        public string? LicenseNumber { get; set; }
        public List<int>? SpecializationIds { get; set; }
        public int? YearsOfExperience { get; set; }
        public decimal? PhoneSessionPrice { get; set; }
        public decimal? InOfficeSessionPrice { get; set; }
    }
}

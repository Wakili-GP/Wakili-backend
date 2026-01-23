using MediatR;
using System.Text.Json.Serialization;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.Update
{
    public class UpdateLawyerCommand : IRequest<Result<LawyerResponse>>
    {
        [JsonIgnore]
        public string Id { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? LicenseNumber { get; set; }
        public string? Specialization { get; set; }
        public int? YearsOfExperience { get; set; }
        public VerificationStatus? VerificationStatus { get; set; }
    }
}

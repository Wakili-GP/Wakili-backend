using System.Collections.Generic;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveBasicInfo;

public class SaveBasicInfoCommand : IRequest<Result<OnboardingStepResponse<BasicInfoDataDto>>>
{
    public string? UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public IFormFile? ProfileImage { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public int? YearsOfExperience { get; set; }
    public List<int> PracticeAreas { get; set; } = new();
    public List<SessionType> SessionTypes { get; set; } = new();
}

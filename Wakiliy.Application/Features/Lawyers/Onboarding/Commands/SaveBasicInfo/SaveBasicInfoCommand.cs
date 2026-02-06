using System.Collections.Generic;
using System.Text.Json.Serialization;
using MediatR;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveBasicInfo;

public class SaveBasicInfoCommand : IRequest<Result<OnboardingStepResponse<BasicInfoDataDto>>>
{
    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;
    public string? ProfileImage { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public int? YearsOfExperience { get; set; }
    public List<int> PracticeAreas { get; set; } = new();
    public List<string> SessionTypes { get; set; } = new();
}

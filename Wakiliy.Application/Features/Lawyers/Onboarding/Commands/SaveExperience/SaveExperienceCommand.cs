using System.Collections.Generic;
using System.Text.Json.Serialization;
using MediatR;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveExperience;

public class SaveExperienceCommand : IRequest<Result<OnboardingStepResponse<ExperienceDataDto>>>
{
    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;

    public List<WorkExperienceDto> WorkExperiences { get; set; } = new();
}

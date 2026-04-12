using System.Collections.Generic;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveExperience;

public class SaveExperienceCommand : IRequest<Result<OnboardingStepResponse<ExperienceDataDto>>>
{
    public string? UserId { get; set; }

    public List<WorkExperienceDto> WorkExperiences { get; set; } = new();
}

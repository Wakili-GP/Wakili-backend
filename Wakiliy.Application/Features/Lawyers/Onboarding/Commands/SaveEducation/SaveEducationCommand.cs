using System.Collections.Generic;
using System.Text.Json.Serialization;
using MediatR;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveEducation;

public class SaveEducationCommand : IRequest<Result<OnboardingStepResponse<EducationDataDto>>>
{
    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;

    public List<AcademicQualificationDto> AcademicQualifications { get; set; } = new();
    public List<ProfessionalCertificationDto>? ProfessionalCertifications { get; set; } = new();
}

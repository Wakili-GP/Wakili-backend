using System.Collections.Generic;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveVerification;

public class SaveVerificationCommand : IRequest<Result<OnboardingStepResponse<VerificationDocumentsDto>>>
{
    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;

    public IFormFile NationalIdFront { get; set; } = default!;
    public IFormFile NationalIdBack { get; set; } = default!;
    public OnBoardingLawyerLicenseDto License { get; set; } = default!;
    public List<IFormFile> EducationalCertificates { get; set; } = new();
    public List<IFormFile> ProfessionalCertificates { get; set; } = new();
}

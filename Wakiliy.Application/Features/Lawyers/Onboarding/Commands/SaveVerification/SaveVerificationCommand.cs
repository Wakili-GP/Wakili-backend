using System.Collections.Generic;
using System.Text.Json.Serialization;
using MediatR;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveVerification;

public class SaveVerificationCommand : IRequest<Result<OnboardingStepResponse<VerificationDocumentsDto>>>
{
    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;

    public UploadedDocumentDto NationalIdFront { get; set; } = new();
    public UploadedDocumentDto NationalIdBack { get; set; } = new();
    public UploadedDocumentDto LawyerLicense { get; set; } = new();
    public List<UploadedDocumentDto> EducationalCertificates { get; set; } = new();
    public List<UploadedDocumentDto> ProfessionalCertificates { get; set; } = new();
}

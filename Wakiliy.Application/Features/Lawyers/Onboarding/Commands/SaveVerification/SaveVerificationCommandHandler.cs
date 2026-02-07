using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Application.Common.Interfaces;
using Wakiliy.Application.Features.Lawyers.Onboarding.Common;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveVerification;

public class SaveVerificationCommandHandler(ILawyerRepository lawyerRepository,IFileUploadService fileUploadService)
    : IRequestHandler<SaveVerificationCommand, Result<OnboardingStepResponse<VerificationDocumentsDto>>>
{
    public async Task<Result<OnboardingStepResponse<VerificationDocumentsDto>>> Handle(SaveVerificationCommand request, CancellationToken cancellationToken)
    {
        //var lawyer = await lawyerRepository.GetLawyerWithVerificationAsync(request.UserId,cancellationToken);

        //if (lawyer is null)
        //    return Result.Failure<OnboardingStepResponse<VerificationDocumentsDto>>(OnboardingErrors.LawyerNotFound);

        //if (!lawyer.CanAccessStep(LawyerOnboardingSteps.Verification))
        //    return Result.Failure<OnboardingStepResponse<VerificationDocumentsDto>>(OnboardingErrors.StepPrerequisite(LawyerOnboardingSteps.Experience));

        //lawyer.VerificationDocuments ??= new VerificationDocuments { LawyerId = lawyer.Id };

        //lawyer.VerificationDocuments.NationalIdFront = UploadAndMap(request.NationalIdFront);

        //if (!TryMapDocument(request.NationalIdBack, out var nationalIdBack))
        //    return Result.Failure<OnboardingStepResponse<VerificationDocumentsDto>>(OnboardingErrors.InvalidDocumentStatus);

        //if (!TryMapDocument(request.LawyerLicense, out var lawyerLicense))
        //    return Result.Failure<OnboardingStepResponse<VerificationDocumentsDto>>(OnboardingErrors.InvalidDocumentStatus);

        //var educationalDocs = new List<UploadedDocument>();
        //foreach (var doc in request.EducationalCertificates)
        //{
        //    if (!TryMapDocument(doc, out var mapped))
        //    {
        //        return Result.Failure<OnboardingStepResponse<VerificationDocumentsDto>>(OnboardingErrors.InvalidDocumentStatus);
        //    }

        //    educationalDocs.Add(mapped);
        //}

        //var professionalDocs = new List<UploadedDocument>();
        //foreach (var doc in request.ProfessionalCertificates)
        //{
        //    if (!TryMapDocument(doc, out var mapped))
        //    {
        //        return Result.Failure<OnboardingStepResponse<VerificationDocumentsDto>>(OnboardingErrors.InvalidDocumentStatus);
        //    }

        //    professionalDocs.Add(mapped);
        //}

        //lawyer.VerificationDocuments.NationalIdFront = nationalIdFront;
        //lawyer.VerificationDocuments.NationalIdBack = nationalIdBack;
        //lawyer.VerificationDocuments.LawyerLicense = lawyerLicense;
        //lawyer.VerificationDocuments.EducationalCertificates = educationalDocs;
        //lawyer.VerificationDocuments.ProfessionalCertificates = professionalDocs;

        //lawyer.VerificationStatus = VerificationStatus.UnderReview;
        //lawyer.MarkStepCompleted(LawyerOnboardingSteps.Verification, LawyerOnboardingSteps.Completed);

        //await lawyerRepository.UpdateAsync(lawyer);

        //var response = LawyerOnboardingHelper.BuildResponse(lawyer, new VerificationDocumentsDto
        //{
        //    NationalIdFront = request.NationalIdFront,
        //    NationalIdBack = request.NationalIdBack,
        //    LawyerLicense = request.LawyerLicense,
        //    EducationalCertificates = request.EducationalCertificates,
        //    ProfessionalCertificates = request.ProfessionalCertificates
        //}, "Documents uploaded for verification");

        return null;
    }

    private async Task<string> UploadAndMap(IFormFile file)
    {
        var result = await fileUploadService.UploadAsync(file, "verification-documents");   
        return result.Url;
    }
    private static bool TryMapDocument(UploadedDocumentDto dto, out UploadedDocument document)
    {
        document = new UploadedDocument
        {
            FilePath = dto.File ?? string.Empty,
            FileName = dto.FileName
        };

        if (Enum.TryParse<DocumentStatus>(dto.Status, true, out var status))
        {
            document.Status = status;
            return true;
        }

        return false;
    }
}

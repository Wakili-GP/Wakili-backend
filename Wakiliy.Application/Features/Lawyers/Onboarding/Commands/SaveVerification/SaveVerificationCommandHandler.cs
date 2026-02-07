using MediatR;
using Microsoft.AspNetCore.Http;
using Wakiliy.Application.Common.Interfaces;
using Wakiliy.Application.Features.Lawyers.Onboarding.Common;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Application.Repositories;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveVerification;

public class SaveVerificationCommandHandler(
    ILawyerRepository lawyerRepository,
    IVerificationDocumentRepository verificationDocumentRepository,
    IUploadedFileRepository uploadedFileRepository,
    IFileUploadService fileUploadService)
    : IRequestHandler<SaveVerificationCommand, Result<OnboardingStepResponse<VerificationDocumentsDto>>>
{
    public async Task<Result<OnboardingStepResponse<VerificationDocumentsDto>>> Handle(
        SaveVerificationCommand request,
        CancellationToken cancellationToken)
    {
        var lawyer = await lawyerRepository.GetByIdAsync(request.UserId);

        if (lawyer is null)
            return Result.Failure<OnboardingStepResponse<VerificationDocumentsDto>>(
                OnboardingErrors.LawyerNotFound);

        // Delete old docs
        await verificationDocumentRepository
            .DeleteByLawyerIdAsync(request.UserId, cancellationToken);

        var documents = new List<VerificationDocuments>();

        // Single files
        documents.Add(await CreateDoc(request.NationalIdFront, request.UserId, VerificationDocumentType.NationalIdFront));
        documents.Add(await CreateDoc(request.NationalIdBack, request.UserId, VerificationDocumentType.NationalIdBack));
        documents.Add(await CreateDoc(request.License.LicenseFile, request.UserId, VerificationDocumentType.LawyerLicense));

        // Multiple files
        documents.AddRange(await CreateMultipleDocs(
            request.EducationalCertificates,
            request.UserId,
            VerificationDocumentType.EducationalCertificate));

        documents.AddRange(await CreateMultipleDocs(
            request.ProfessionalCertificates,
            request.UserId,
            VerificationDocumentType.ProfessionalCertificate));

        lawyer.VerificationDocuments = documents;
        lawyer.LicenseNumber = request.License.LicenseNumber;
        lawyer.LicenseYear = request.License.LicenseYear;
        lawyer.IssuingAuthority = request.License.IssuingAuthority;

        lawyer.MarkStepCompleted(
            LawyerOnboardingSteps.Verification,
            LawyerOnboardingSteps.Completed);

        await lawyerRepository.UpdateAsync(lawyer,cancellationToken);

        return Result.Success(LawyerOnboardingHelper.BuildResponse(lawyer,PopulateResponsed(lawyer),"Verification documents saved"));
    }


    private async Task<VerificationDocuments> CreateDoc(
        IFormFile file,
        string userId,
        VerificationDocumentType type)
    {
        var uploaded = await SaveAndUploadFileAsync(file, userId);

        return new VerificationDocuments
        {
            LawyerId = userId,
            FileId = uploaded.Id,
            File = uploaded,
            Type = type
        };
    }

    private async Task<List<VerificationDocuments>> CreateMultipleDocs(
        List<IFormFile> files,
        string userId,
        VerificationDocumentType type)
    {
        var result = new List<VerificationDocuments>();

        foreach (var file in files)
            result.Add(await CreateDoc(file, userId, type));

        return result;
    }

    private async Task<UploadedFile> SaveAndUploadFileAsync(
        IFormFile file,
        string userId)
    {
        var upload = await fileUploadService.UploadAsync(file, "uploads");

        var entity = new UploadedFile
        {
            Id = Guid.NewGuid(),
            OwnerId = userId,
            FileName = upload.FileName,
            PublicId = upload.PublicId,
            Size = upload.Size,
            ContentType = upload.ContentType,
            Purpose = FilePurpose.Verification
        };

        entity.SystemFileUrl = $"/api/files/{entity.Id}";

        await uploadedFileRepository.AddAsync(entity, CancellationToken.None);
        return entity;
    }

    private VerificationDocumentsDto PopulateResponsed(Lawyer lawyer)
    {
        return new VerificationDocumentsDto
        {
            NationalIdFront = lawyer.VerificationDocuments.FirstOrDefault(d => d.Type == VerificationDocumentType.NationalIdFront)?.File?.SystemFileUrl,
            NationalIdBack = lawyer.VerificationDocuments.FirstOrDefault(d => d.Type == VerificationDocumentType.NationalIdBack)?.File?.SystemFileUrl,
            LawyerLicense = new LawyerLicenseDto
            {
                LicensePath = lawyer.VerificationDocuments.FirstOrDefault(d => d.Type == VerificationDocumentType.LawyerLicense)?.File?.SystemFileUrl,
                LicenseNumber = lawyer.LicenseNumber,
                LicenseYear = lawyer.LicenseYear,
                IssuingAuthority = lawyer.IssuingAuthority
            },
            EducationalCertificates = lawyer.VerificationDocuments
                .Where(d => d.Type == VerificationDocumentType.EducationalCertificate)
                .Select(d => d.File?.SystemFileUrl ?? string.Empty)
                .ToList(),
            ProfessionalCertificates = lawyer.VerificationDocuments
                .Where(d => d.Type == VerificationDocumentType.ProfessionalCertificate)
                .Select(d => d.File?.SystemFileUrl ?? string.Empty)
                .ToList()
        };
    }
}

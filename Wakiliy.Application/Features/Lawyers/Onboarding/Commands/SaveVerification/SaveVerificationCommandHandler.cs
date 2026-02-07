using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Application.Common.Interfaces;
using Wakiliy.Application.Features.Lawyers.Onboarding.Common;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Application.Repositories;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
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

        // 🧹 Delete old docs
        await verificationDocumentRepository
            .DeleteByLawyerIdAsync(request.UserId, cancellationToken);

        var documents = new List<VerificationDocuments>();

        // Single files
        documents.Add(await CreateDoc(request.NationalIdFront, request.UserId, VerificationDocumentType.NationalIdFront));
        documents.Add(await CreateDoc(request.NationalIdBack, request.UserId, VerificationDocumentType.NationalIdBack));
        documents.Add(await CreateDoc(request.LawyerLicense, request.UserId, VerificationDocumentType.LawyerLicense));

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

        lawyer.MarkStepCompleted(
            LawyerOnboardingSteps.Verification,
            LawyerOnboardingSteps.Completed);

        await lawyerRepository.UpdateAsync(lawyer);

        return Result.Success(
            LawyerOnboardingHelper.BuildResponse(
                lawyer,
                new VerificationDocumentsDto(),
                "Verification documents saved"));
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
}

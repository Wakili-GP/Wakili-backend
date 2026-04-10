using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Wakiliy.Application.Common.Interfaces;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Application.Repositories;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Commands.UpdateLawyerInfo
{
    public class UpdateLawyerInfoCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateLawyerInfoCommandHandler> logger, IFileUploadService fileUploadService) : IRequestHandler<UpdateLawyerInfoCommand, Result<UserInfoResponse>>
    {
        public async Task<Result<UserInfoResponse>> Handle(UpdateLawyerInfoCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("UpdateLawyerInfoCommandHandler: {Id}", request.Id);
            var lawyer = await unitOfWork.Lawyers.GetByIdAsync(request.Id, cancellationToken);
            if (lawyer is null)
            {
                return Result.Failure<UserInfoResponse>(new Error("Lawyer.NotFound", "Lawyer profile not found or user is not a lawyer", StatusCodes.Status404NotFound));
            }

            if (request.SpecializationIds is not null)
            {
                var ids = request.SpecializationIds.Distinct().ToList();
                var specializations = await unitOfWork.Specializations.GetByIdsAsync(ids, cancellationToken);
                if (specializations.Count != ids.Count)
                {
                    return Result.Failure<UserInfoResponse>(SpecializationErrors.InvalidSelection);
                }

                lawyer.Specializations.Clear();
                foreach (var specialization in specializations)
                {
                    lawyer.Specializations.Add(specialization);
                }
            }

            
            lawyer.FirstName = request.FirstName ?? lawyer.FirstName;
            lawyer.LastName = request.LastName ?? lawyer.LastName;
            lawyer.PhoneNumber = request.PhoneNumber ?? lawyer.PhoneNumber;
            lawyer.City = request.City ?? lawyer.City;
            lawyer.Country = request.Country ?? lawyer.Country;
            lawyer.Gender = request.Gender ?? lawyer.Gender;
            lawyer.LicenseNumber = request.LicenseNumber ?? lawyer.LicenseNumber;
            lawyer.YearsOfExperience = request.YearsOfExperience ?? lawyer.YearsOfExperience;
            lawyer.PhoneSessionPrice = request.PhoneSessionPrice ?? lawyer.PhoneSessionPrice;
            lawyer.InOfficeSessionPrice = request.InOfficeSessionPrice ?? lawyer.InOfficeSessionPrice;

            var response = lawyer.Adapt<UserInfoResponse>();
            response.UserType = DefaultRoles.Lawyer;

            if (request.ProfileImage is not null)
            {
                var existingFiles = await unitOfWork.UploadedFiles.GetByOwnerAsync(lawyer.Id, FilePurpose.Profile, cancellationToken);
                var existingProfileImages = existingFiles.Where(f => f.Category == FileCategory.ProfilePicture).ToList();
                foreach (var existingFile in existingProfileImages)
                {
                    await unitOfWork.UploadedFiles.DeleteAsync(existingFile, cancellationToken);
                }

                var uploadResult = await fileUploadService.UploadAsync(request.ProfileImage, "uploads");
                var file = new UploadedFile
                {
                    Id = Guid.NewGuid(),
                    OwnerId = lawyer.Id,
                    FileName = uploadResult.FileName,
                    PublicId = uploadResult.PublicId,
                    Size = uploadResult.Size,
                    ContentType = uploadResult.ContentType,
                    Category = FileCategory.ProfilePicture,
                    Purpose = FilePurpose.Profile,
                    UploadedAt = DateTime.UtcNow
                };

                file.SystemFileUrl = $"/api/files/{file.Id}";
                response.profileImage = file.SystemFileUrl;
                lawyer.ProfileImage = file;
                await unitOfWork.UploadedFiles.AddAsync(file, cancellationToken);
                
            }
            await unitOfWork.Lawyers.UpdateAsync(lawyer, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(response);
        }
    }
}

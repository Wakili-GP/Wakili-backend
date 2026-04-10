using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Wakiliy.Application.Common.Interfaces;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Application.Repositories;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Commands.UpdateClientInfo
{
    public class UpdateClientInfoCommandHandler(IUnitOfWork unitOfWork, IFileUploadService fileUploadService) : IRequestHandler<UpdateClientInfoCommand, Result<UserInfoResponse>>
    {
        public async Task<Result<UserInfoResponse>> Handle(UpdateClientInfoCommand request, CancellationToken cancellationToken)
        {
            var client = await unitOfWork.Clients.GetByIdAsync(request.Id, cancellationToken);
            if (client is null)
            {
                return Result.Failure<UserInfoResponse>(new Error("Client.NotFound", "Client profile not found or user is not a client", StatusCodes.Status404NotFound));
            }

            client.FirstName = request.FirstName ?? client.FirstName;
            client.LastName = request.LastName ?? client.LastName;
            client.PhoneNumber = request.PhoneNumber ?? client.PhoneNumber;
            client.City = request.City ?? client.City;
            client.Country = request.Country ?? client.Country;
            client.Bio = request.Bio ?? client.Bio;

            var response = client.Adapt<UserInfoResponse>();
            response.UserType = DefaultRoles.Client;
            response.IsEmailVerified = client.EmailConfirmed;
            response.LastName = client.LastName;
            response.FirstName = client.FirstName;


            if (request.ProfileImage is not null)
            {
                var existingFiles = await unitOfWork.UploadedFiles.GetByOwnerAsync(client.Id, FilePurpose.Profile, cancellationToken);
                var existingProfileImages = existingFiles.Where(f => f.Category == FileCategory.ProfilePicture).ToList();
                foreach (var existingFile in existingProfileImages)
                {
                    await unitOfWork.UploadedFiles.DeleteAsync(existingFile, cancellationToken);
                }

                var uploadResult = await fileUploadService.UploadAsync(request.ProfileImage, "uploads");
                var file = new UploadedFile
                {
                    Id = Guid.NewGuid(),
                    OwnerId = client.Id,
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
                client.ProfileImage = file;
                await unitOfWork.UploadedFiles.AddAsync(file, cancellationToken);

            }
            else
            {
                response.profileImage = client.ProfileImage?.SystemFileUrl;
            }

            await unitOfWork.Clients.UpdateAsync(client, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Success(response);
        }
    }
}

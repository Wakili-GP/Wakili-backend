using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Application.Common.Interfaces;
using Wakiliy.Application.Common.Models;

namespace Wakiliy.Infrastructure.Services
{
    public class CloudinaryFileUploadService : IFileUploadService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryFileUploadService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<FileUploadResult> UploadAsync(IFormFile file,string folder)
        {
            if (file.Length == 0)
                throw new ArgumentException("File is empty");

            await using var stream = file.OpenReadStream();

            var isPdf = file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) ||
                        file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);

            RawUploadParams uploadParams;
            if (isPdf)
            {
                uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder,
                    UseFilename = false,
                    UniqueFilename = true,
                    Overwrite = false
                };
            }
            else
            {
                uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder,
                    UseFilename = false,
                    UniqueFilename = true,
                    Overwrite = false
                };
            }

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.StatusCode != HttpStatusCode.OK)
                throw new Exception(result.Error?.Message);


            return new FileUploadResult
            {
                Url = result.SecureUrl.ToString(),
                PublicId = result.PublicId,
                FileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length
            };
        }
    }

}

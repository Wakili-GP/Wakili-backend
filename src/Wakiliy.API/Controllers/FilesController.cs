using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Repositories;
using Wakiliy.Infrastructure.Repositories;
using Wakiliy.Domain.Constants;

namespace Wakiliy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController(IUploadedFileRepository uploadedFileRepository, IConfiguration configuration) : ControllerBase
    {

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFile(Guid id)
        {
            var file = await uploadedFileRepository.GetByIdAsync(id);

            if (file is null)
                return NotFound();

            var cloudName = configuration["Cloudinary:CloudName"];

            var extension = Path.GetExtension(file.FileName);
            var publicIdWithExtension = file.PublicId;
            if (!string.IsNullOrEmpty(extension) && !publicIdWithExtension.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            {
                publicIdWithExtension += extension;
            }

            var isPdf = file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) ||
                        file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);

            using var httpClient = new HttpClient();
            HttpResponseMessage response;

            if (isPdf)
            {
                // Try fetching as raw first (newly uploaded PDFs)
                var rawUrl = $"https://res.cloudinary.com/{cloudName}/raw/upload/{publicIdWithExtension}";
                response = await httpClient.GetAsync(rawUrl);

                // Fallback to image/upload if raw/upload returns 404 (backward compatibility for old PDFs)
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var imageUrl = $"https://res.cloudinary.com/{cloudName}/image/upload/{publicIdWithExtension}";
                    response = await httpClient.GetAsync(imageUrl);
                }
            }
            else
            {
                var imageUrl = $"https://res.cloudinary.com/{cloudName}/image/upload/{publicIdWithExtension}";
                response = await httpClient.GetAsync(imageUrl);
            }

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var stream = await response.Content.ReadAsStreamAsync();

            Response.Headers["Content-Disposition"] = $"inline; filename=\"{file.FileName}\"";

            return new FileStreamResult(stream, file.ContentType)
            {
                EnableRangeProcessing = true
            };
        }

    }
}

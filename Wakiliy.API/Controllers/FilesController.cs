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

            // if (file.OwnerId != User.GetUserId() && !User.IsInRole(DefaultRoles.Admin)) return Forbid();

            var cloudName = configuration["Cloudinary:CloudName"];

            var cloudinaryUrl =
                $"https://res.cloudinary.com/{cloudName}/raw/upload/{file.PublicId}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(cloudinaryUrl);

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var stream = await response.Content.ReadAsStreamAsync();
            var contentType = file.ContentType ?? "application/octet-stream";


            Response.Headers["Content-Disposition"] = $"inline; filename=\"{file.FileName}\"";

            return new FileStreamResult(stream, contentType)
            {
                EnableRangeProcessing = true 
            };
        }

    }
}

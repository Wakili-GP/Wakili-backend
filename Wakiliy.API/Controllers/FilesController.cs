using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Repositories;
using Wakiliy.Infrastructure.Repositories;

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

            if (file.OwnerId != User.GetUserId()) return Forbid();

            var cloudName = configuration["Cloudinary:CloudName"];

            var cloudinaryUrl =
                $"https://res.cloudinary.com/{cloudName}/raw/upload/{file.PublicId}";

            return Redirect(cloudinaryUrl);
        }

    }
}

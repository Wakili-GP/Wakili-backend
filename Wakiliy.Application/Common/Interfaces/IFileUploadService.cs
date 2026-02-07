using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Application.Common.Models;

namespace Wakiliy.Application.Common.Interfaces
{
    public interface IFileUploadService
    {
        Task<FileUploadResult> UploadAsync(IFormFile file,string folder);
    }
}

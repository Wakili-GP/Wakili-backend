using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wakiliy.Application.Common.Settings
{
    public class CloudinarySettings
    {
        public static string SectionName = "Cloudinary";
        public string CloudName { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
        public string ApiSecret { get; set; } = null!;
    }
}

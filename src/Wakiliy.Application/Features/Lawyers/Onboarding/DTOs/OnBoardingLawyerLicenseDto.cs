using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.DTOs
{
    public class OnBoardingLawyerLicenseDto
    {
        public IFormFile LicenseFile { get; set; } = default!;
        public string LicenseNumber { get; set; } = string.Empty;
        public string IssuingAuthority { get; set; } = string.Empty;
        public string LicenseYear { get; set; } = string.Empty;
    }
}

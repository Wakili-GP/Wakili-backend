using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Domain.Enums;

namespace Wakiliy.Application.Features.Lawyers.DTOs
{
    public class LawyerResponse
    {
        public string Id { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int? YearsOfExperience { get; set; }
        public bool IsActive { get; set; }
        public VerificationStatus? VerificationStatus { get; set; }
        public DateTime JoinedDate { get; set; }
    }
}

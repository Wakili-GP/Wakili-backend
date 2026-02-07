using System;
using System.Collections.Generic;
namespace Wakiliy.Application.Features.Lawyers.DTOs
{
    public class LawyerVerificationRequestResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public List<string> Specialties { get; set; } = new();
        public DateTime SubmittedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public List<VerificationDocumentResponse> Documents { get; set; } = new();
    }

    public class VerificationDocumentResponse
    {
        public string Type { get; set; } = string.Empty;
        public string? FileUrl { get; set; }
    }
}

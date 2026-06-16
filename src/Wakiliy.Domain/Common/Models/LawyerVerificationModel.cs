namespace Wakiliy.Domain.Common.Models
{
    public class LawyerVerificationModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Specializations { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectedBy { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }

       public class LawyerVerificationStats
        {
            public int Total { get; set; }
            public int Pending { get; set; }
            public int UnderReview { get; set; }
            public int Approved { get; set; }
            public int Rejected { get; set; }
        }
}
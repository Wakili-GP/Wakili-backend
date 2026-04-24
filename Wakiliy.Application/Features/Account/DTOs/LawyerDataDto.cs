namespace Wakiliy.Application.Features.Account.DTOs
{
    public class LawyerDataDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Bio { get; set; }
        public string? Summary { get; set; }
        public decimal? PhoneSessionPrice { get; set; }
        public decimal? InOfficeSessionPrice { get; set; }
        public string ProfileImage { get; set; } = string.Empty;
        public DateTime? MemberSince { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}

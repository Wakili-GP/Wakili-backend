namespace Wakiliy.Application.Features.Account.DTOs
{
    public class ClientDataDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty;
        public string MemberSince { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}

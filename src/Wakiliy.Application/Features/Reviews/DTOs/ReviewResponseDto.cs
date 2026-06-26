namespace Wakiliy.Application.Features.Reviews.DTOs;

public class ReviewClientDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public string? Bio { get; set; }
}

public class ReviewLawyerDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
}

public class ReviewResponseDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string LawyerId { get; set; } = string.Empty;
    public Guid AppointmentId { get; set; }
    public double Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? AiComment { get; set; }
    public double? AiConfidenceRate { get; set; }
    public DateTime? AiProcessedAt { get; set; }
    public string AiStatus { get; set; } = string.Empty;
    public string Visibility { get; set; } = string.Empty;
    
    public ReviewClientDto Client { get; set; } = new();
    public ReviewLawyerDto Lawyer { get; set; } = new();
}
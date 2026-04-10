namespace Wakiliy.Application.Features.Reviews.DTOs;

public class ReviewResponseDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string LawyerId { get; set; } = string.Empty;
    public Guid AppointmentId { get; set; }
    public double Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public AiReviewDto AiAnalysis { get; set; } = new();
}

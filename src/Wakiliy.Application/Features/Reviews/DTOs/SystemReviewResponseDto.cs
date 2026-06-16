namespace Wakiliy.Application.Features.Reviews.DTOs;

public class SystemReviewResponseDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public AiReviewDto AiAnalysis { get; set; } = new();
}

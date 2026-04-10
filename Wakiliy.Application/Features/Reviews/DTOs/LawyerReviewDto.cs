namespace Wakiliy.Application.Features.Reviews.DTOs;

public class LawyerReviewDto
{
    public double Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public AiReviewDto AiReview { get; set; } = new();
}

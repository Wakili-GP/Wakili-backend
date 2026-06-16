namespace Wakiliy.Application.Features.Reviews.DTOs;

public class AiReviewDto
{
    public bool IsFlagged { get; set; }
    public double Confidence { get; set; }
    public string Summary { get; set; } = string.Empty;
}

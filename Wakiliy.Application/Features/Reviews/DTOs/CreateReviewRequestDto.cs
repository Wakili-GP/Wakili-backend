namespace Wakiliy.Application.Features.Reviews.DTOs;

public class CreateReviewRequestDto
{
    public string AppointmentId { get; set; } = string.Empty;
    public LawyerReviewDto LawyerReview { get; set; } = new();
    public SystemReviewDto? SystemReview { get; set; }
}

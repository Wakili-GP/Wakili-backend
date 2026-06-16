namespace Wakiliy.Application.Features.Reviews.DTOs;

public class LawyerReviewStatsDto
{
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public Dictionary<int, int> StarCounts { get; set; } = new();
}

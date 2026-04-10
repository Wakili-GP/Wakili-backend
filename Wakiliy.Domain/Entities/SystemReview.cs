namespace Wakiliy.Domain.Entities;

public class SystemReview
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // AI analysis (owned entity)
    public AiAnalysis AiAnalysis { get; set; } = new();

    // Navigation property
    public Client User { get; set; } = default!;
}

namespace Wakiliy.Domain.Entities;

public class Review
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string LawyerId { get; set; } = string.Empty;
    public Guid AppointmentId { get; set; }
    public double Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // AI analysis (owned entity)
    public AiAnalysis AiAnalysis { get; set; } = new();

    // Navigation properties
    public Appointment Appointment { get; set; } = default!;
    public Client User { get; set; } = default!;
    public Lawyer Lawyer { get; set; } = default!;
}

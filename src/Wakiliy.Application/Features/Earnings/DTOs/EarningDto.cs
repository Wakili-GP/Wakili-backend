namespace Wakiliy.Application.Features.Earnings.DTOs;

public class EarningDto
{
    public int Id { get; set; }
    public Guid AppointmentId { get; set; }
    public string LawyerId { get; set; } = string.Empty;
    public string LawyerName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public decimal GrossAmount { get; set; }
    public decimal PlatformFee { get; set; }
    public decimal NetAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int? PayrollId { get; set; }
}

namespace Wakiliy.Application.Features.Payrolls.DTOs;

public class PayrollDto
{
    public int Id { get; set; }
    public string LawyerId { get; set; } = string.Empty;
    public string LawyerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}

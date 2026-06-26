namespace Wakiliy.Application.Features.Earnings.DTOs;

public class LawyerEarningsSummaryDto
{
    public decimal AvailableBalance { get; set; }
    public decimal PendingBalance { get; set; }
    public decimal PaidBalance { get; set; }
    public decimal TotalEarnings { get; set; }
    public List<EarningDto> Earnings { get; set; } = new();
}

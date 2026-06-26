using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Entities;

public class Payroll
{
    public int Id { get; set; }
    public string LawyerId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public PayrollStatus Status { get; set; } = PayrollStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }

    public Lawyer Lawyer { get; set; } = default!;
    public ICollection<LawyerEarning> Earnings { get; set; } = new List<LawyerEarning>();
}

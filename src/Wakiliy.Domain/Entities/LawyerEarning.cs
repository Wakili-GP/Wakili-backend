using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Entities;

public class LawyerEarning
{
    public int Id { get; set; }
    public Guid AppointmentId { get; set; }
    public string LawyerId { get; set; } = string.Empty;
    public decimal GrossAmount { get; set; }
    public decimal PlatformFee { get; set; }
    public decimal NetAmount { get; set; }
    public LawyerEarningStatus Status { get; set; } = LawyerEarningStatus.Pending;
    public int? PayrollId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Appointment Appointment { get; set; } = default!;
    public Lawyer Lawyer { get; set; } = default!;
    public Payroll? Payroll { get; set; }
}

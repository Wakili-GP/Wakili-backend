using System;
using Wakiliy.Domain.Common;

namespace Wakiliy.Domain.Entities
{
    public class AppointmentSlot : AuditableEntity
    {
        public int Id { get; set; }
        public string LawyerId { get; set; } = null!;
        public Lawyer? Lawyer { get; set; }
        
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}

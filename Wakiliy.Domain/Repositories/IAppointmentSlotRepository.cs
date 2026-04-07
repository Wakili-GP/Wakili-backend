using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Domain.Repositories;

public interface IAppointmentSlotRepository
{
    Task<AppointmentSlot?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    IQueryable<AppointmentSlot> GetByLawyerIdQuery(string lawyerId);
    Task AddAsync(AppointmentSlot appointmentSlot, CancellationToken cancellationToken = default);
    Task UpdateAsync(AppointmentSlot appointmentSlot, CancellationToken cancellationToken = default);
    Task DeleteAsync(AppointmentSlot appointmentSlot, CancellationToken cancellationToken = default);
    Task<bool> HasOverlappingSlotAsync(string lawyerId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, int? excludeSlotId = null, CancellationToken cancellationToken = default);
}

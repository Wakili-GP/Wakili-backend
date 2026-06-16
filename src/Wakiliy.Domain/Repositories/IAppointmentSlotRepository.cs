using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Repositories;

public interface IAppointmentSlotRepository
{
    Task<AppointmentSlot?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    IQueryable<AppointmentSlot> GetByLawyerIdQuery(string lawyerId,DateOnly date,SessionType? sessionType);
    Task AddAsync(AppointmentSlot appointmentSlot, CancellationToken cancellationToken = default);
    Task UpdateAsync(AppointmentSlot appointmentSlot, CancellationToken cancellationToken = default);
    Task DeleteAsync(AppointmentSlot appointmentSlot, CancellationToken cancellationToken = default);
    Task<bool> HasOverlappingSlotAsync(string lawyerId, DateOnly date, TimeSpan startTime, TimeSpan endTime, int? excludeSlotId = null, CancellationToken cancellationToken = default);
}

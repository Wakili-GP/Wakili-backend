using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories;

internal class AppointmentSlotRepository(ApplicationDbContext dbContext) : IAppointmentSlotRepository
{
    public async Task<AppointmentSlot?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.AppointmentSlots
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public IQueryable<AppointmentSlot> GetByLawyerIdQuery(string lawyerId)
    {
        return dbContext.AppointmentSlots
            .AsNoTracking()
            .Where(x => x.LawyerId == lawyerId)
            .OrderBy(x => x.DayOfWeek)
            .ThenBy(x => x.StartTime)
            .AsQueryable();
    }

    public async Task AddAsync(AppointmentSlot appointmentSlot, CancellationToken cancellationToken = default)
    {
        await dbContext.AppointmentSlots.AddAsync(appointmentSlot, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(AppointmentSlot appointmentSlot, CancellationToken cancellationToken = default)
    {
        dbContext.AppointmentSlots.Update(appointmentSlot);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(AppointmentSlot appointmentSlot, CancellationToken cancellationToken = default)
    {
        dbContext.AppointmentSlots.Remove(appointmentSlot);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasOverlappingSlotAsync(string lawyerId, System.DayOfWeek dayOfWeek, System.TimeSpan startTime, System.TimeSpan endTime, int? excludeSlotId = null, CancellationToken cancellationToken = default)
    {
        return await dbContext.AppointmentSlots
            .AnyAsync(x => x.LawyerId == lawyerId 
                        && x.DayOfWeek == dayOfWeek 
                        && (!excludeSlotId.HasValue || x.Id != excludeSlotId)
                        && x.StartTime < endTime 
                        && x.EndTime > startTime, 
                      cancellationToken);
    }
}

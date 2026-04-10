using Microsoft.EntityFrameworkCore;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories;

internal class AppointmentRepository(ApplicationDbContext dbContext) : IAppointmentRepository
{
    public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Appointments
            .Include(a => a.Slot)
            .Include(a => a.Client)
            .Include(a => a.Lawyer)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Appointment>> GetByClientIdAsync(string clientId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Appointments
            .AsNoTracking()
            .Include(a => a.Slot)
            .Include(a => a.Lawyer)
                .ThenInclude(l=>l.ProfileImage)
            .Where(a => a.ClientId == clientId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Appointment>> GetByLawyerIdAsync(string lawyerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Appointments
            .AsNoTracking()
            .Include(a => a.Slot)
            .Include(a => a.Client)
                .ThenInclude(c => c.ProfileImage)
            .Where(a => a.LawyerId == lawyerId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsSlotBookedAsync(int slotId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Appointments
            .AnyAsync(a => a.SlotId == slotId 
                && a.Status != Domain.Enums.AppointmentStatus.Cancelled, cancellationToken);
    }

    public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        await dbContext.Appointments.AddAsync(appointment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        dbContext.Appointments.Update(appointment);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

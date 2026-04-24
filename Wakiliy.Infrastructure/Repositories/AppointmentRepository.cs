using Microsoft.EntityFrameworkCore;
using Wakiliy.Domain.Common.Models;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
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

    public async Task<(IEnumerable<LawyerReceivedAppointmentModel> Requests, int TotalCount, AppointmentsReceivedStats Stats)> GetByLawyerPagedAsync(
        string lawyerId,
        int page,
        int pageSize,
        string? search,
        AppointmentStatus? status,
        bool sortDescending,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Appointments
            .AsNoTracking()
            .Include(a => a.Slot)
            .Include(a => a.Client)
                .ThenInclude(c => c.ProfileImage)
            .Where(a => a.LawyerId == lawyerId);

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowerSearch = search.ToLower();
            query = query.Where(a =>
                a.Client.FirstName.ToLower().Contains(lowerSearch) ||
                a.Client.LastName.ToLower().Contains(lowerSearch));
        }

        var totalCount = await query.CountAsync(cancellationToken);

         query = sortDescending
                ? query.OrderByDescending(a=>a.CreatedAt)
                : query.OrderBy(a => a.CreatedAt);

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new LawyerReceivedAppointmentModel
            {
                Id = l.Id,
                Status = l.Status,
                CreatedAt = l.CreatedAt,
                ConfirmedAt = l.ConfirmedAt,
                CancelledAt = l.CancelledAt,
                CompletedAt = l.CompletedAt,
                SessionDate = l.Slot.Date,
                StartTime = l.Slot.StartTime,
                EndTime = l.Slot.EndTime,
                SessionType = l.Slot.SessionType,
                ClientId = l.ClientId,
                ClientFirstName = l.Client.FirstName,
                ClientLastName = l.Client.LastName,
                ClientPhone = l.Client.PhoneNumber,
                ClientProfileImage = l.Client.ProfileImage != null ? l.Client.ProfileImage.SystemFileUrl : null,
            })
            .ToListAsync(cancellationToken);

        var stats = await GetVerificationStatsAsync(cancellationToken);

        return (result, totalCount, stats);
        
    }

    public async Task<AppointmentsReceivedStats> GetVerificationStatsAsync(CancellationToken cancellationToken = default)
        {
            var stats = await dbContext.Appointments
                .GroupBy(l => 1)
                .Select(g => new AppointmentsReceivedStats
                {
                    Total = g.Count(),
                    Pending = g.Count(l => l.Status == AppointmentStatus.Pending),
                    Confirmed = g.Count(l => l.Status == AppointmentStatus.Confirmed),
                    Cancelled = g.Count(l => l.Status == AppointmentStatus.Cancelled),
                    Completed = g.Count(l => l.Status == AppointmentStatus.Completed)
                })
                .FirstOrDefaultAsync(cancellationToken);

            return stats ?? new AppointmentsReceivedStats();
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

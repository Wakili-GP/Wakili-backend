using Microsoft.EntityFrameworkCore;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories;

internal class LawyerEarningRepository(ApplicationDbContext dbContext) : ILawyerEarningRepository
{
    public async Task AddAsync(LawyerEarning earning, CancellationToken cancellationToken = default)
    {
        await dbContext.LawyerEarnings.AddAsync(earning, cancellationToken);
    }

    public async Task<List<LawyerEarning>> GetAllEarningsAsync(string? lawyerId, LawyerEarningStatus? status, DateTime? dateFrom, DateTime? dateTo, string? search, CancellationToken cancellationToken = default)
    {
        var query = dbContext.LawyerEarnings
            .Include(e => e.Lawyer)
            .Include(e => e.Appointment)
                .ThenInclude(a => a.Client)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(lawyerId))
            query = query.Where(e => e.LawyerId == lawyerId);

        if (status.HasValue)
            query = query.Where(e => e.Status == status.Value);

        if (dateFrom.HasValue)
            query = query.Where(e => e.CreatedAt >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(e => e.CreatedAt <= dateTo.Value);

        if (!string.IsNullOrEmpty(search))
        {
            var s = search.ToLower();
            query = query.Where(e => 
                (e.Lawyer.FirstName + " " + e.Lawyer.LastName).ToLower().Contains(s) ||
                (e.Appointment.Client.FirstName + " " + e.Appointment.Client.LastName).ToLower().Contains(s));
        }

        return await query.OrderByDescending(e => e.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<List<LawyerEarning>> GetLawyerEarningsAsync(string lawyerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.LawyerEarnings
            .Include(e => e.Appointment)
                .ThenInclude(a => a.Client)
            .AsNoTracking()
            .Where(e => e.LawyerId == lawyerId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<LawyerEarning>> GetPendingEarningsAsync(string lawyerId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var query = dbContext.LawyerEarnings
            .Include(e => e.Appointment)
                .ThenInclude(a => a.Client)
            .Include(e => e.Lawyer)
            .Where(e => e.LawyerId == lawyerId && e.Status == LawyerEarningStatus.Pending && e.PayrollId == null);

        if (fromDate.HasValue)
            query = query.Where(e => e.CreatedAt >= fromDate.Value);
            
        if (toDate.HasValue)
            query = query.Where(e => e.CreatedAt <= toDate.Value);

        return await query.ToListAsync(cancellationToken);
    }
}

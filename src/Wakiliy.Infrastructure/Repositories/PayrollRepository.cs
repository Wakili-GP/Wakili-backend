using Microsoft.EntityFrameworkCore;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories;

internal class PayrollRepository(ApplicationDbContext dbContext) : IPayrollRepository
{
    public async Task AddAsync(Payroll payroll, CancellationToken cancellationToken = default)
    {
        await dbContext.Payrolls.AddAsync(payroll, cancellationToken);
    }

    public async Task<List<Payroll>> GetAllPayrollsAsync(string? lawyerId, PayrollStatus? status, DateTime? dateFrom, DateTime? dateTo, string? search, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Payrolls
            .Include(p => p.Lawyer)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(lawyerId))
            query = query.Where(p => p.LawyerId == lawyerId);

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        if (dateFrom.HasValue)
            query = query.Where(p => p.CreatedAt >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(p => p.CreatedAt <= dateTo.Value);

        if (!string.IsNullOrEmpty(search))
        {
            var s = search.ToLower();
            query = query.Where(p => (p.Lawyer.FirstName + " " + p.Lawyer.LastName).ToLower().Contains(s));
        }

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<Payroll?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Payrolls.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Payroll?> GetByIdWithEarningsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Payrolls
            .Include(p => p.Lawyer)
            .Include(p => p.Earnings)
                .ThenInclude(e => e.Appointment)
                    .ThenInclude(a => a.Client)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}

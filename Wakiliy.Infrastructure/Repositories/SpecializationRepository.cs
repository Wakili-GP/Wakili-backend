using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories;

internal class SpecializationRepository(ApplicationDbContext dbContext) : ISpecializationRepository
{
    public async Task<Specialization?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Specializations
            .Include(s => s.Lawyers)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<List<Specialization>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Specializations
            .AsNoTracking()
            .OrderByDescending(s => s.CreatedOn)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Specialization>> GetActiveAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Specializations
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Specialization>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
        {
            return new List<Specialization>();
        }

        return await dbContext.Specializations
            .Where(s => idList.Contains(s.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId, CancellationToken cancellationToken)
    {
        return await dbContext.Specializations
            .AnyAsync(s => s.Name == name && (!excludeId.HasValue || s.Id != excludeId), cancellationToken);
    }

    public async Task AddAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        await dbContext.Specializations.AddAsync(specialization, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        dbContext.Specializations.Update(specialization);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        dbContext.Specializations.Remove(specialization);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

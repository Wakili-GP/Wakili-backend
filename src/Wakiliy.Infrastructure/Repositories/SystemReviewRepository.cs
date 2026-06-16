using Microsoft.EntityFrameworkCore;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories;

internal class SystemReviewRepository(ApplicationDbContext dbContext) : ISystemReviewRepository
{
    public async Task AddAsync(SystemReview systemReview, CancellationToken cancellationToken = default)
    {
        await dbContext.SystemReviews.AddAsync(systemReview, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsFirstReviewForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return !await dbContext.SystemReviews
            .AnyAsync(r => r.UserId == userId, cancellationToken);
    }

    public async Task<List<SystemReview>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SystemReviews
            .AsNoTracking()
            .Include(sr => sr.User)
            .OrderByDescending(sr => sr.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

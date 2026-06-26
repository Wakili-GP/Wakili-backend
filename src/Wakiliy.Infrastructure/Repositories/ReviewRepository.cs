using Microsoft.EntityFrameworkCore;
using Wakiliy.Domain.Common.Models;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;
using Wakiliy.Domain.Enums;

namespace Wakiliy.Infrastructure.Repositories;

internal class ReviewRepository(ApplicationDbContext dbContext) : IReviewRepository
{
    public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        await dbContext.Reviews.AddAsync(review, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Reviews
            .AnyAsync(r => r.AppointmentId == appointmentId, cancellationToken);
    }

    public async Task<List<Review>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Reviews
            .AsNoTracking()
            .Include(r => r.User)
                .ThenInclude(u => u.ProfileImage)
            .Include(r => r.Lawyer)
                .ThenInclude(l => l.ProfileImage)
            .Include(r => r.Appointment)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Review>> GetByLawyerIdAsync(string lawyerId, double? stars = null, CancellationToken cancellationToken = default)
    {
        return await dbContext.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Appointment)
            .Where(r => r.LawyerId == lawyerId &&
                        r.Visibility == ReviewVisibility.Visible &&
                        (!stars.HasValue || r.Rating >= stars.Value))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Review> Reviews, int TotalCount)> GetByLawyerIdPagedAsync(
        string lawyerId,
        int page,
        int pageSize,
        double? stars = null,
        string? searchQuery = null,
        bool sortDescending = true,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .ThenInclude(u => u.ProfileImage)
            .Include(r => r.Appointment)
            .Where(r => r.LawyerId == lawyerId &&
                        r.Visibility == ReviewVisibility.Visible &&
                        (!stars.HasValue || r.Rating >= stars.Value));

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(r => 
                r.User.FirstName.Contains(searchQuery) ||
                r.User.LastName.Contains(searchQuery) ||
                r.Comment.Contains(searchQuery));
        }

        query = sortDescending ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (reviews, totalCount);
    }

    public async Task<LawyerReviewStatsModel> GetLawyerReviewStatsAsync(string lawyerId, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Reviews
            .AsNoTracking()
            .Where(r => r.LawyerId == lawyerId && r.Visibility == ReviewVisibility.Visible);

        var stats = await query
            .GroupBy(_ => 1)
            .Select(g => new LawyerReviewStatsModel
            {
                AverageRating = g.Average(r => r.Rating),
                TotalReviews = g.Count(),
                OneStarCount = g.Count(r => r.Rating <= 1),
                TwoStarCount = g.Count(r => r.Rating > 1 && r.Rating <= 2),
                ThreeStarCount = g.Count(r => r.Rating > 2 && r.Rating <= 3),
                FourStarCount = g.Count(r => r.Rating > 3 && r.Rating <= 4),
                FiveStarCount = g.Count(r => r.Rating > 4 && r.Rating <= 5)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return stats ?? new LawyerReviewStatsModel();
    }


    public async Task<Review?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Reviews
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.AppointmentId == appointmentId, cancellationToken);
    }

    public async Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Reviews
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
    {
        dbContext.Reviews.Update(review);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

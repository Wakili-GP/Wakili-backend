using Microsoft.EntityFrameworkCore;
using Wakiliy.Domain.Common.Models;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

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
            .Include(r => r.Lawyer)
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
                        !r.AiAnalysis.IsFlagged &&
                        (!stars.HasValue || r.Rating >= stars.Value))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Review> Reviews, int TotalCount)> GetByLawyerIdPagedAsync(
        string lawyerId,
        int page,
        int pageSize,
        double? stars = null,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Appointment)
            .Where(r => r.LawyerId == lawyerId &&
                        !r.AiAnalysis.IsFlagged &&
                        (!stars.HasValue || r.Rating >= stars.Value));

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
            .Where(r => r.LawyerId == lawyerId && !r.AiAnalysis.IsFlagged);

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
}

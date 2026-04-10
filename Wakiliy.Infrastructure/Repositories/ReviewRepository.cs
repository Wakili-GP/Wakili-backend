using Microsoft.EntityFrameworkCore;
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

    public async Task<List<Review>> GetByLawyerIdAsync(string lawyerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Appointment)
            .Where(r => r.LawyerId == lawyerId && !r.AiAnalysis.IsFlagged)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

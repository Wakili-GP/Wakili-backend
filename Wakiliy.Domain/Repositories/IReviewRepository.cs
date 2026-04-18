using Wakiliy.Domain.Common.Models;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Domain.Repositories;

public interface IReviewRepository
{
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    Task<bool> ExistsByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<List<Review>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Review>> GetByLawyerIdAsync(string lawyerId, double? stars = null, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Review> Reviews, int TotalCount)> GetByLawyerIdPagedAsync(
        string lawyerId,
        int page,
        int pageSize,
        double? stars = null,
        CancellationToken cancellationToken = default);
    Task<LawyerReviewStatsModel> GetLawyerReviewStatsAsync(string lawyerId, CancellationToken cancellationToken = default);
}

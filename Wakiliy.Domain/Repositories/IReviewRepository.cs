using Wakiliy.Domain.Entities;

namespace Wakiliy.Domain.Repositories;

public interface IReviewRepository
{
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    Task<bool> ExistsByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<List<Review>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Review>> GetByLawyerIdAsync(string lawyerId, CancellationToken cancellationToken = default);
}

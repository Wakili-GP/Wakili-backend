using Wakiliy.Domain.Entities;

namespace Wakiliy.Domain.Repositories;

public interface ISystemReviewRepository
{
    Task AddAsync(SystemReview systemReview, CancellationToken cancellationToken = default);
    Task<bool> IsFirstReviewForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<SystemReview>> GetAllAsync(CancellationToken cancellationToken = default);
}

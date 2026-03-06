using Wakiliy.Domain.Entities;

namespace Wakiliy.Domain.Repositories;

public interface IFavoriteLawyerRepository
{
    Task<bool> ExistsAsync(string userId, string lawyerId, CancellationToken cancellationToken = default);
    Task AddAsync(FavoriteLawyer favoriteLawyer, CancellationToken cancellationToken = default);
    Task RemoveAsync(string userId, string lawyerId, CancellationToken cancellationToken = default);
    Task<List<Lawyer>> GetFavoriteLawyersAsync(string userId, CancellationToken cancellationToken = default);
}

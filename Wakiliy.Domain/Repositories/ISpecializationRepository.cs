using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Domain.Repositories;

public interface ISpecializationRepository
{
    Task<Specialization?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Specialization>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Specialization>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<List<Specialization>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Specialization specialization, CancellationToken cancellationToken = default);
    Task UpdateAsync(Specialization specialization, CancellationToken cancellationToken = default);
    Task DeleteAsync(Specialization specialization, CancellationToken cancellationToken = default);
}

using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Repositories;

public interface ILawyerEarningRepository
{
    Task AddAsync(LawyerEarning earning, CancellationToken cancellationToken = default);
    Task<List<LawyerEarning>> GetAllEarningsAsync(string? lawyerId, LawyerEarningStatus? status, DateTime? dateFrom, DateTime? dateTo, string? search, CancellationToken cancellationToken = default);
    Task<List<LawyerEarning>> GetLawyerEarningsAsync(string lawyerId, CancellationToken cancellationToken = default);
    Task<List<LawyerEarning>> GetPendingEarningsAsync(string lawyerId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
}

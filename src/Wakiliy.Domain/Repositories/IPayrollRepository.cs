using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Repositories;

public interface IPayrollRepository
{
    Task<Payroll?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Payroll?> GetByIdWithEarningsAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Payroll>> GetAllPayrollsAsync(string? lawyerId, PayrollStatus? status, DateTime? dateFrom, DateTime? dateTo, string? search, CancellationToken cancellationToken = default);
    Task AddAsync(Payroll payroll, CancellationToken cancellationToken = default);
}

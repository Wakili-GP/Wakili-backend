using Wakiliy.Domain.Common.Models;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Repositories;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Appointment>> GetByClientIdAsync(string clientId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<LawyerReceivedAppointmentModel> Requests, int TotalCount, AppointmentsReceivedStats Stats)> GetByLawyerPagedAsync(
        string lawyerId,
        int page,
        int pageSize,
        string? search,
        AppointmentStatus? status,
        bool sortDescending,
        CancellationToken cancellationToken = default);
    Task<bool> IsSlotBookedAsync(int slotId, CancellationToken cancellationToken = default);
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task<List<ApprovedAppointmentModel>> GetApprovedAppointmentsAsync(string lawyerId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
}

using Wakiliy.Domain.Entities;

namespace Wakiliy.Domain.Repositories;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Appointment>> GetByClientIdAsync(string clientId, CancellationToken cancellationToken = default);
    Task<List<Appointment>> GetByLawyerIdAsync(string lawyerId, CancellationToken cancellationToken = default);
    Task<bool> IsSlotBookedAsync(int slotId, CancellationToken cancellationToken = default);
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default);
}

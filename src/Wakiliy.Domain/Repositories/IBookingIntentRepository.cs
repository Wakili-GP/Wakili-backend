using Wakiliy.Domain.Entities;

namespace Wakiliy.Domain.Repositories;

public interface IBookingIntentRepository
{
    Task AddAsync(BookingIntent bookingIntent, CancellationToken cancellationToken = default);
    Task<BookingIntent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Update(BookingIntent bookingIntent);
}
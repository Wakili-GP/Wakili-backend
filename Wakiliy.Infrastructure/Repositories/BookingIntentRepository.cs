using Microsoft.EntityFrameworkCore;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories;

public class BookingIntentRepository(ApplicationDbContext dbContext) : IBookingIntentRepository
{
    public async Task AddAsync(BookingIntent bookingIntent, CancellationToken cancellationToken = default)
    {
        await dbContext.BookingIntents.AddAsync(bookingIntent, cancellationToken);
    }

    public async Task<BookingIntent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.BookingIntents
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void Update(BookingIntent bookingIntent)
    {
        dbContext.BookingIntents.Update(bookingIntent);
    }
}
using MediatR;
using Microsoft.AspNetCore.Http;
using Wakiliy.Application.Interfaces.Services;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.BookingIntents.Commands.Create;

public class CreateBookingIntentCommandHandler(IUnitOfWork unitOfWork, IPaymobService paymobService) 
    : IRequestHandler<CreateBookingIntentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateBookingIntentCommand request, CancellationToken cancellationToken)
    {
        var slot = await unitOfWork.AppointmentSlots.GetByIdAsync(request.SlotId, cancellationToken);
        if (slot is null)
            return Result.Failure<string>(AppointmentErrors.SlotNotFound);

        if (request.ClientId == request.LawyerId)
            return Result.Failure<string>(AppointmentErrors.CannotBookOwnSlot);

        var isBooked = await unitOfWork.Appointments.IsSlotBookedAsync(request.SlotId, cancellationToken);
        if (isBooked)
            return Result.Failure<string>(AppointmentErrors.SlotAlreadyBooked);

        // Fetch Lawyer / Client objects for pricing and info 
        var lawyer = await unitOfWork.Lawyers.GetByIdAsync(request.LawyerId, cancellationToken);
        if (lawyer is null)
            return Result.Failure<string>(AppointmentErrors.LawyerNotFound);

        var amount = slot.SessionType == SessionType.Phone ? (lawyer.PhoneSessionPrice ?? 0) : (lawyer.InOfficeSessionPrice ?? 0);
        if (amount == 0)
        {
            // If the session is free, you might want to just book it directly. Assuming paid here.
            return Result.Failure<string>(new Error("Booking.InvalidPrice", "The session price is invalid.",StatusCodes.Status400BadRequest));
        }

        var intent = new BookingIntent
        {
            Id = Guid.NewGuid(),
            SlotId = request.SlotId,
            ClientId = request.ClientId,
            LawyerId = request.LawyerId,
            Amount = amount,
            Status = BookingIntentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };

        var client = await unitOfWork.Clients.GetByIdAsync(request.ClientId, cancellationToken);

        if(client is null)
            return Result.Failure<string>(AppointmentErrors.Unauthorized);

        var paymentKey = await paymobService.CreatePaymentKeyAsync(
            amount, client?.Email, client.FirstName, client.LastName, client.PhoneNumber!, intent.Id.ToString());

        await unitOfWork.BookingIntents.AddAsync(intent, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(paymentKey);
    }
}
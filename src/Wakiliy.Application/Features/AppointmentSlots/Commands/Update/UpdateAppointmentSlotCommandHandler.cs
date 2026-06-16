using MediatR;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.AppointmentSlots.Commands.Update;

public class UpdateAppointmentSlotCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateAppointmentSlotCommand, Result>
{
    public async Task<Result> Handle(UpdateAppointmentSlotCommand request, CancellationToken cancellationToken)
    {
        var appointmentSlot = await unitOfWork.AppointmentSlots.GetByIdAsync(request.Id, cancellationToken);

        if (appointmentSlot is null)
            return Result.Failure(new Error("AppointmentSlot.NotFound", "The appointment slot was not found.", 404));

        var hasOverlap = await unitOfWork.AppointmentSlots.HasOverlappingSlotAsync(
            request.LawyerId, request.Date, request.StartTime, request.EndTime, request.Id, cancellationToken);

        if (hasOverlap)
            return Result.Failure(new Error("AppointmentSlot.Overlap", "The appointment slot overlaps with an existing slot.", 409));

        appointmentSlot.Date = request.Date;
        appointmentSlot.StartTime = request.StartTime;
        appointmentSlot.EndTime = request.EndTime;
        appointmentSlot.SessionType = request.SessionType;

        await unitOfWork.AppointmentSlots.UpdateAsync(appointmentSlot, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
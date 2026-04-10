using MediatR;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.AppointmentSlots.Commands.Update;

public class UpdateAppointmentSlotCommandHandler(IAppointmentSlotRepository appointmentSlotRepository) 
    : IRequestHandler<UpdateAppointmentSlotCommand, Result>
{
    public async Task<Result> Handle(UpdateAppointmentSlotCommand request, CancellationToken cancellationToken)
    {
        var appointmentSlot = await appointmentSlotRepository.GetByIdAsync(request.Id, cancellationToken);

        if (appointmentSlot is null)
        {
            return Result.Failure(new Error("AppointmentSlot.NotFound", "The appointment slot was not found.", 404));
        }

        var hasOverlap = await appointmentSlotRepository.HasOverlappingSlotAsync(
            request.LawyerId, request.Date, request.StartTime, request.EndTime, request.Id, cancellationToken);
            
        if (hasOverlap)
        {
            return Result.Failure(new Error("AppointmentSlot.Overlap", "The appointment slot overlaps with an existing slot.", 409));
        }

        appointmentSlot.Date = request.Date;
        appointmentSlot.StartTime = request.StartTime;
        appointmentSlot.EndTime = request.EndTime;
        appointmentSlot.SessionType = request.SessionType;

        await appointmentSlotRepository.UpdateAsync(appointmentSlot, cancellationToken);

        return Result.Success();
    }
}
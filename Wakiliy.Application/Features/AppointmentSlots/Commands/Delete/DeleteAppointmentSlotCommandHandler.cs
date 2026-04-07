using MediatR;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.AppointmentSlots.Commands.Delete;

public class DeleteAppointmentSlotCommandHandler(IAppointmentSlotRepository appointmentSlotRepository) 
    : IRequestHandler<DeleteAppointmentSlotCommand, Result>
{
    public async Task<Result> Handle(DeleteAppointmentSlotCommand request, CancellationToken cancellationToken)
    {
        var appointmentSlot = await appointmentSlotRepository.GetByIdAsync(request.Id, cancellationToken);

        if (appointmentSlot is null)
        {
            return Result.Failure(new Error("AppointmentSlot.NotFound", "The appointment slot was not found.", 404));
        }

        await appointmentSlotRepository.DeleteAsync(appointmentSlot, cancellationToken);

        return Result.Success();
    }
}
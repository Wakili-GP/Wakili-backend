using MediatR;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Commands.Confirm;

public class ConfirmAppointmentCommandHandler(IAppointmentRepository appointmentRepository)
    : IRequestHandler<ConfirmAppointmentCommand, Result>
{
    public async Task<Result> Handle(ConfirmAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);

        if (appointment is null)
            return Result.Failure(AppointmentErrors.AppointmentNotFound);

        if (appointment.LawyerId != request.LawyerId)
            return Result.Failure(AppointmentErrors.Unauthorized);

        if (appointment.Status != AppointmentStatus.Pending)
            return Result.Failure(AppointmentErrors.InvalidStatusTransition);

        appointment.Status = AppointmentStatus.Confirmed;
        appointment.ConfirmedAt = DateTime.UtcNow;

        await appointmentRepository.UpdateAsync(appointment, cancellationToken);

        return Result.Success();
    }
}

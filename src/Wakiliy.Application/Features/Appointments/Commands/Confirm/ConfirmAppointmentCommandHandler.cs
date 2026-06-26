using MediatR;
using Wakiliy.Application.Interfaces.Services;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Commands.Confirm;

public class ConfirmAppointmentCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    : IRequestHandler<ConfirmAppointmentCommand, Result>
{
    public async Task<Result> Handle(ConfirmAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, cancellationToken);

        if (appointment is null)
            return Result.Failure(AppointmentErrors.AppointmentNotFound);

        if (appointment.LawyerId != request.LawyerId)
            return Result.Failure(AppointmentErrors.Unauthorized);

        if (appointment.Status != AppointmentStatus.Pending)
            return Result.Failure(AppointmentErrors.InvalidStatusTransition);

        appointment.Status = AppointmentStatus.Confirmed;
        appointment.ConfirmedAt = DateTime.UtcNow;

        await unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await notificationService.SendNotificationAsync(
            userId: appointment.ClientId,
            title: "تم تأكيد موعدك",
            message: "تم تأكيد موعدك من قِبل المحامي.",
            type: NotificationType.AppointmentConfirmed,
            referenceId: appointment.Id.ToString(),
            cancellationToken: cancellationToken);

        return Result.Success();
    }
}


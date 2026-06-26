using MediatR;
using Wakiliy.Application.Interfaces.Services;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Commands.Reject;

public class RejectAppointmentCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    : IRequestHandler<RejectAppointmentCommand, Result>
{
    public async Task<Result> Handle(RejectAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, cancellationToken);

        if (appointment is null)
            return Result.Failure(AppointmentErrors.AppointmentNotFound);

        if (appointment.LawyerId != request.LawyerId)
            return Result.Failure(AppointmentErrors.Unauthorized);

        if (appointment.Status == AppointmentStatus.Completed || appointment.Status == AppointmentStatus.Cancelled)
            return Result.Failure(AppointmentErrors.InvalidStatusTransition);

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.CancelledAt = DateTime.UtcNow;

        await unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await notificationService.SendNotificationAsync(
            userId: appointment.ClientId,
            title: "تم رفض موعدك",
            message: "للأسف تم رفض طلب موعدك من قِبل المحامي.",
            type: NotificationType.AppointmentRejected,
            referenceId: appointment.Id.ToString(),
            cancellationToken: cancellationToken);

        return Result.Success();
    }
}


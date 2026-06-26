using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Wakiliy.Application.Helpers;
using Wakiliy.Application.Interfaces.Services;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Features.Appointments.Commands.Complete;

public class CompleteAppointmentCommandHandler(
    IUnitOfWork unitOfWork,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor,
    INotificationService notificationService)
    : IRequestHandler<CompleteAppointmentCommand, Result>
{
    public async Task<Result> Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, cancellationToken);

        if (appointment is null)
            return Result.Failure(AppointmentErrors.AppointmentNotFound);

        if (appointment.LawyerId != request.LawyerId)
            return Result.Failure(AppointmentErrors.Unauthorized);

        if (appointment.Status != AppointmentStatus.Confirmed)
            return Result.Failure(AppointmentErrors.InvalidStatusTransition);

        appointment.Status = AppointmentStatus.Completed;
        appointment.CompletedAt = DateTime.UtcNow;

        var grossAmount = appointment.PaymentTransaction?.Amount ?? 0;
        var platformFee = grossAmount * 0.20m;
        var netAmount = grossAmount - platformFee;

        var earning = new LawyerEarning
        {
            AppointmentId = appointment.Id,
            LawyerId = appointment.LawyerId,
            GrossAmount = grossAmount,
            PlatformFee = platformFee,
            NetAmount = netAmount,
            Status = LawyerEarningStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.LawyerEarnings.AddAsync(earning, cancellationToken);
        await unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Send review invitation email to the client
        await SendReviewInvitationEmailAsync(
            appointment.Client.Email!,
            appointment.Client.FirstName,
            appointment.Lawyer.FirstName + " " + appointment.Lawyer.LastName,
            appointment.Id);

        await notificationService.SendNotificationAsync(
            userId: appointment.ClientId,
            title: "اكتمل موعدك",
            message: "تم إتمام جلستك بنجاح. شاركنا تقييمك للمحامي.",
            type: NotificationType.AppointmentCompleted,
            referenceId: appointment.Id.ToString(),
            cancellationToken: cancellationToken);

        return Result.Success();
    }

    private async Task SendReviewInvitationEmailAsync(string clientEmail, string clientFirstName, string lawyerFullName, Guid appointmentId)
    {
        var origin = httpContextAccessor.HttpContext?.Request.Headers.Origin.ToString();
        var reviewUrl = $"{origin}/appointments/{appointmentId}/review";

        var tokens = new Dictionary<string, string>
        {
            { "{{clientName}}", clientFirstName },
            { "{{lawyerName}}", lawyerFullName },
            { "{{reviewUrl}}", reviewUrl }
        };

        var emailBody = EmailBodyBuilder.GenerateEmailBody("ReviewInvitation", tokens);
        await emailSender.SendEmailAsync(clientEmail, "قيّم تجربتك مع المحامي ⭐", emailBody);
    }
}


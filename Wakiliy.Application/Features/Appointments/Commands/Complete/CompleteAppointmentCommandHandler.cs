using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Wakiliy.Application.Helpers;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Commands.Complete;

public class CompleteAppointmentCommandHandler(
    IUnitOfWork unitOfWork,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor)
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

        await unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Send review invitation email to the client
        await SendReviewInvitationEmailAsync(
            appointment.Client.Email!,
            appointment.Client.FirstName,
            appointment.Lawyer.FirstName + " " + appointment.Lawyer.LastName,
            appointment.Id);

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

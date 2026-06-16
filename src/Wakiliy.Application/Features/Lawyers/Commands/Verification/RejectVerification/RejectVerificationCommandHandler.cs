using MediatR;
using Microsoft.AspNetCore.Identity.UI.Services;
using Wakiliy.Application.Features.Lawyers.Onboarding.Common;
using Wakiliy.Application.Helpers;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.Verification.RejectVerification;

public class RejectVerificationCommandHandler(
    IUnitOfWork unitOfWork,
    IEmailSender emailSender) : IRequestHandler<RejectVerificationCommand, Result>
{
    public async Task<Result> Handle(RejectVerificationCommand request, CancellationToken cancellationToken)
    {
        var lawyer = await unitOfWork.Lawyers.GetByIdAsync(request.LawyerId, cancellationToken);

        if (lawyer is null)
            return Result.Failure(OnboardingErrors.LawyerNotFound);

        if (lawyer.VerificationStatus == VerificationStatus.Rejected)
            return Result.Failure(OnboardingErrors.AlreadyRejected);

        lawyer.VerificationStatus = VerificationStatus.Rejected;
        lawyer.RejectedById = request.AdminId;
        lawyer.RejectedAt = DateTime.UtcNow;
        lawyer.ApprovedAt = null;
        lawyer.ApprovedById = null;

        lawyer.MarkStepCompleted(
            LawyerOnboardingSteps.Verification,
            LawyerOnboardingSteps.PendingReview);

        await unitOfWork.Lawyers.UpdateAsync(lawyer, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var noteSection = string.IsNullOrWhiteSpace(request.Note)
            ? string.Empty
            : $"<div class=\"note-box\"><strong>ملاحظة المراجع:</strong><br/>{request.Note}</div>";

        var tokens = new Dictionary<string, string>
        {
            { "{{name}}", $"{lawyer.FirstName} {lawyer.LastName}" },
            { "{{noteSection}}", noteSection }
        };

        var emailBody = EmailBodyBuilder.GenerateEmailBody("VerificationRejected", tokens);
        await emailSender.SendEmailAsync(lawyer.Email!, "بخصوص طلب التحقق الخاص بك", emailBody);

        return Result.Success();
    }
}

using MediatR;
using Microsoft.AspNetCore.Identity.UI.Services;
using Wakiliy.Application.Helpers;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.Verification.RejectVerification;

public class RejectVerificationCommandHandler(
    ILawyerRepository lawyerRepository,
    IEmailSender emailSender) : IRequestHandler<RejectVerificationCommand, Result>
{
    public async Task<Result> Handle(RejectVerificationCommand request, CancellationToken cancellationToken)
    {
        var lawyer = await lawyerRepository.GetByIdAsync(request.LawyerId, cancellationToken);

        if (lawyer is null)
            return Result.Failure(OnboardingErrors.LawyerNotFound);

        if (lawyer.VerificationStatus == VerificationStatus.Rejected)
            return Result.Failure(OnboardingErrors.AlreadyRejected);

        lawyer.VerificationStatus = VerificationStatus.Rejected;
        await lawyerRepository.UpdateAsync(lawyer, cancellationToken);

        // Build the optional note section HTML
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

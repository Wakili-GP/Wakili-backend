using MediatR;
using Microsoft.AspNetCore.Identity.UI.Services;
using Wakiliy.Application.Helpers;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.Verification.ApproveVerification;

public class ApproveVerificationCommandHandler(
    ILawyerRepository lawyerRepository,
    IEmailSender emailSender) : IRequestHandler<ApproveVerificationCommand, Result>
{
    public async Task<Result> Handle(ApproveVerificationCommand request, CancellationToken cancellationToken)
    {
        var lawyer = await lawyerRepository.GetByIdAsync(request.LawyerId, cancellationToken);

        if (lawyer is null)
            return Result.Failure(OnboardingErrors.LawyerNotFound);

        if (lawyer.VerificationStatus == VerificationStatus.Approved)
            return Result.Failure(OnboardingErrors.AlreadyApproved);

        lawyer.VerificationStatus = VerificationStatus.Approved;
        await lawyerRepository.UpdateAsync(lawyer, cancellationToken);

        // Send approval email
        var tokens = new Dictionary<string, string>
        {
            { "{{name}}", $"{lawyer.FirstName} {lawyer.LastName}" }
        };

        var emailBody = EmailBodyBuilder.GenerateEmailBody("VerificationApproved", tokens);
        await emailSender.SendEmailAsync(lawyer.Email!, "تمت الموافقة على طلب التحقق الخاص بك", emailBody);

        return Result.Success();
    }
}

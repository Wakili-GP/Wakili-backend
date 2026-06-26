using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using Wakiliy.Application.Common.Interfaces;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Repositories;
using Microsoft.AspNetCore.Identity.UI.Services;
using Wakiliy.Application.Helpers;

namespace Wakiliy.Application.BackgroundJobs;

public class LawyerVerificationJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiLawyerVerificationService _aiLawyerVerificationService;
    private readonly ILogger<LawyerVerificationJob> _logger;
    private readonly IEmailSender _emailSender;

    public LawyerVerificationJob(
        IUnitOfWork unitOfWork,
        IAiLawyerVerificationService aiLawyerVerificationService,
        ILogger<LawyerVerificationJob> logger,
        IEmailSender emailSender)
    {
        _unitOfWork = unitOfWork;
        _aiLawyerVerificationService = aiLawyerVerificationService;
        _logger = logger;
        _emailSender = emailSender;
    }

    [AutomaticRetry(Attempts = 5)]
    public async Task ProcessVerificationAsync(string lawyerId)
    {
        _logger.LogInformation("LawyerVerificationJob started for lawyer {LawyerId}", lawyerId);

        var lawyer = await _unitOfWork.Lawyers.GetByIdAsync(lawyerId);
        if (lawyer is null)
        {
            _logger.LogWarning("Lawyer {LawyerId} not found, skipping verification.", lawyerId);
            return;
        }

        if (lawyer.VerificationStatus != VerificationStatus.UnderReview)
        {
            _logger.LogInformation("Lawyer {LawyerId} is not UnderReview (current status: {Status}), skipping verification.", lawyerId, lawyer.VerificationStatus);
            return;
        }

        try
        {
            _logger.LogInformation("Calling AI Lawyer Verification service for lawyer {LawyerId}", lawyerId);
            
            if (!Guid.TryParse(lawyer.Id, out var lawyerGuid))
            {
                 _logger.LogWarning("LawyerId {LawyerId} is not a valid GUID. Skipping.", lawyerId);
                 return;
            }

            var aiResult = await _aiLawyerVerificationService.VerifyLawyerAsync(lawyerGuid);

            if (aiResult is null)
            {
                _logger.LogWarning("AI Lawyer Verification returned null for lawyer {LawyerId}. Will retry.", lawyerId);
                throw new Exception("AI Verification result is null.");
            }

            _logger.LogInformation("AI Lawyer Verification for {LawyerId} completed. Status: {Status}, Reason: {Reason}", lawyerId, aiResult.Status, aiResult.Reason);

            if (aiResult.IsValid)
            {
                lawyer.VerificationStatus = VerificationStatus.Approved;
                lawyer.ApprovedAt = DateTime.UtcNow;
                _logger.LogInformation("Lawyer {LawyerId} automatically marked as Approved by AI.", lawyerId);

                if (!string.IsNullOrEmpty(lawyer.Email))
                {
                    var model = new Dictionary<string, string>
                    {
                        { "{{name}}", $"{lawyer.FirstName} {lawyer.LastName}" }
                    };
                    var body = EmailBodyBuilder.GenerateEmailBody("LawyerVerificationApproved", model);
                    await _emailSender.SendEmailAsync(lawyer.Email, "تمت الموافقة على طلب التحقق", body);
                }
            }
            else
            {
                lawyer.VerificationStatus = VerificationStatus.Rejected;
                lawyer.RejectedAt = DateTime.UtcNow;
                // Currently Lawyer does not have a RejectionReason property, but we could log the reason or save it if added.
                // For now we just mark as rejected.
                _logger.LogInformation("Lawyer {LawyerId} automatically marked as Rejected by AI. Reason: {Reason}", lawyerId, aiResult.Reason);

                if (!string.IsNullOrEmpty(lawyer.Email))
                {
                    var model = new Dictionary<string, string>
                    {
                        { "{{name}}", $"{lawyer.FirstName} {lawyer.LastName}" },
                        { "{{noteSection}}", $"<div class=\"note-box\">{aiResult.Reason}</div>" }
                    };
                    var body = EmailBodyBuilder.GenerateEmailBody("LawyerVerificationRejected", model);
                    await _emailSender.SendEmailAsync(lawyer.Email, "تم رفض طلب التحقق", body);
                }
            }
            
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("LawyerVerificationJob successfully completed for lawyer {LawyerId}", lawyerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process AI lawyer verification for lawyer {LawyerId}", lawyerId);
            
            // Re-throw so Hangfire can retry
            throw;
        }
    }
}

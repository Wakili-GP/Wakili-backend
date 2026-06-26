using Hangfire;
using MediatR;
using Wakiliy.Application.Features.Lawyers.Onboarding.Common;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Application.BackgroundJobs;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SubmitForReview;

public class SubmitForReviewCommandHandler(
    IUnitOfWork unitOfWork,
    IBackgroundJobClient backgroundJobClient) 
    : IRequestHandler<SubmitForReviewCommand, Result>
{
    public async Task<Result> Handle(SubmitForReviewCommand request, CancellationToken cancellationToken)
    {
        var lawyer = await unitOfWork.Lawyers.GetByIdAsync(request.UserId, cancellationToken);

        if (lawyer is null)
            return Result.Failure(OnboardingErrors.LawyerNotFound);

        bool allStepsCompleted = lawyer.HasCompletedStep(LawyerOnboardingSteps.BasicInfo)
            && lawyer.HasCompletedStep(LawyerOnboardingSteps.Education)
            && lawyer.HasCompletedStep(LawyerOnboardingSteps.Experience)
            && lawyer.HasCompletedStep(LawyerOnboardingSteps.Verification);

        if (!allStepsCompleted)
            return Result.Failure(
                new Error("Onboarding.Incomplete", "All steps must be completed before submitting for review.", 400));

        lawyer.MarkStepCompleted(LawyerOnboardingSteps.PendingReview, -1);
        lawyer.CurrentOnboardingStep = -1;

        lawyer.VerificationStatus = VerificationStatus.UnderReview;

        await unitOfWork.Lawyers.UpdateAsync(lawyer, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Enqueue background job for AI lawyer verification
        backgroundJobClient.Enqueue<LawyerVerificationJob>(x => x.ProcessVerificationAsync(lawyer.Id));

        return Result.Success();
    }
}

using Hangfire;
using Microsoft.Extensions.Logging;
using Wakiliy.Application.Common.Interfaces;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Repositories;

namespace Wakiliy.Application.BackgroundJobs;

public class ReviewModerationJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiReviewAnalysisService _aiReviewAnalysisService;
    private readonly ILogger<ReviewModerationJob> _logger;

    public ReviewModerationJob(
        IUnitOfWork unitOfWork,
        IAiReviewAnalysisService aiReviewAnalysisService,
        ILogger<ReviewModerationJob> logger)
    {
        _unitOfWork = unitOfWork;
        _aiReviewAnalysisService = aiReviewAnalysisService;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 5)]
    public async Task ProcessReviewAsync(Guid reviewId)
    {
        _logger.LogInformation("ReviewModerationJob started for review {ReviewId}", reviewId);

        var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
        if (review is null)
        {
            _logger.LogWarning("Review {ReviewId} not found, skipping moderation.", reviewId);
            return;
        }

        if (review.AiStatus == ReviewAiStatus.Completed)
        {
            _logger.LogInformation("Review {ReviewId} is already Completed, skipping moderation.", reviewId);
            return;
        }

        review.AiStatus = ReviewAiStatus.Processing;
        await _unitOfWork.SaveChangesAsync();

        try
        {
            _logger.LogInformation("Calling AI Review Analysis service for review {ReviewId}", reviewId);
            var aiResult = await _aiReviewAnalysisService.AnalyzeReviewAsync(review.Comment, review.Rating);

            _logger.LogInformation("AI Review Analysis for {ReviewId} completed. Flag: {Flag}, Confidence: {Confidence}", reviewId, aiResult.Flag, aiResult.ConfidenceRate);

            review.AiComment = aiResult.Comment;
            review.AiConfidenceRate = aiResult.ConfidenceRate;
            review.AiProcessedAt = DateTime.UtcNow;
            review.AiStatus = ReviewAiStatus.Completed;

            if (string.Equals(aiResult.Flag, "Visible", StringComparison.OrdinalIgnoreCase))
            {
                review.Visibility = ReviewVisibility.Visible;
                _logger.LogInformation("Review {ReviewId} marked as Visible.", reviewId);
            }
            else
            {
                review.Visibility = ReviewVisibility.Hidden;
                _logger.LogInformation("Review {ReviewId} marked as Hidden based on AI flag.", reviewId);
            }
            
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("ReviewModerationJob successfully completed for review {ReviewId}", reviewId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process AI review moderation for review {ReviewId}", reviewId);
            
            review.AiStatus = ReviewAiStatus.Failed;
            // Ensure visibility remains Pending or whatever it was
            review.Visibility = ReviewVisibility.Pending; 
            review.AiComment = $"Moderation failed: {ex.Message}";
            
            await _unitOfWork.SaveChangesAsync();

            // Re-throw so Hangfire can retry
            throw;
        }
    }
}

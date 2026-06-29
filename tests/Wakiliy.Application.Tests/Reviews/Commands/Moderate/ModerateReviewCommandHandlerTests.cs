using FluentAssertions;
using Hangfire;
using Moq;
using Wakiliy.Application.Features.Reviews.Commands.Moderate;
using Wakiliy.Application.BackgroundJobs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Repositories;

namespace Wakiliy.Application.Tests.Reviews.Commands.Moderate;

public class ModerateReviewCommandHandlerTests
{
    private readonly Mock<IReviewRepository> _reviewRepoMock;
    private readonly Mock<IBackgroundJobClient> _backgroundJobClientMock;
    private readonly ModerateReviewCommandHandler _handler;

    public ModerateReviewCommandHandlerTests()
    {
        _reviewRepoMock = new Mock<IReviewRepository>();
        _backgroundJobClientMock = new Mock<IBackgroundJobClient>();

        _handler = new ModerateReviewCommandHandler(
            _reviewRepoMock.Object,
            _backgroundJobClientMock.Object);
    }

    [Fact]
    public async Task Handle_WhenReviewNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new ModerateReviewCommand { ReviewId = Guid.NewGuid(), Action = ReviewModerationAction.Approve };
        _reviewRepoMock.Setup(r => r.GetByIdAsync(command.ReviewId, default)).ReturnsAsync((Review)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.code.Should().Be("ReviewNotFound");
    }

    [Fact]
    public async Task Handle_WithRetryModerationAction_UpdatesStatusAndEnqueuesJob()
    {
        // Arrange
        var review = new Review { Id = Guid.NewGuid(), AiStatus = ReviewAiStatus.Completed, Visibility = ReviewVisibility.Visible };
        var command = new ModerateReviewCommand { ReviewId = review.Id, Action = ReviewModerationAction.RetryModeration };
        
        _reviewRepoMock.Setup(r => r.GetByIdAsync(command.ReviewId, default)).ReturnsAsync(review);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        review.AiStatus.Should().Be(ReviewAiStatus.Pending);
        review.Visibility.Should().Be(ReviewVisibility.Pending);
        
        _reviewRepoMock.Verify(r => r.UpdateAsync(review, default), Times.Once);
        _backgroundJobClientMock.Verify(x => x.Create(
            It.Is<Hangfire.Common.Job>(job => job.Type == typeof(ReviewModerationJob) && job.Method.Name == nameof(ReviewModerationJob.ProcessReviewAsync)),
            It.IsAny<Hangfire.States.EnqueuedState>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithApproveAction_UpdatesVisibilityToVisible()
    {
        // Arrange
        var review = new Review { Id = Guid.NewGuid(), Visibility = ReviewVisibility.Pending };
        var command = new ModerateReviewCommand { ReviewId = review.Id, Action = ReviewModerationAction.Approve };
        
        _reviewRepoMock.Setup(r => r.GetByIdAsync(command.ReviewId, default)).ReturnsAsync(review);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        review.Visibility.Should().Be(ReviewVisibility.Visible);
        
        _reviewRepoMock.Verify(r => r.UpdateAsync(review, default), Times.Once);
    }

    [Fact]
    public async Task Handle_WithHideAction_UpdatesVisibilityToHidden()
    {
        // Arrange
        var review = new Review { Id = Guid.NewGuid(), Visibility = ReviewVisibility.Pending };
        var command = new ModerateReviewCommand { ReviewId = review.Id, Action = ReviewModerationAction.Hide };
        
        _reviewRepoMock.Setup(r => r.GetByIdAsync(command.ReviewId, default)).ReturnsAsync(review);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        review.Visibility.Should().Be(ReviewVisibility.Hidden);
        
        _reviewRepoMock.Verify(r => r.UpdateAsync(review, default), Times.Once);
    }
}

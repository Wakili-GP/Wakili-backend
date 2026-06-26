using FluentAssertions;
using Moq;
using Wakiliy.Application.Features.Reviews.Commands.Create;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Application.Interfaces.Services;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;

namespace Wakiliy.Application.Tests.Reviews.Commands.Create;

public class CreateReviewCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IReviewRepository> _reviewRepoMock;
    private readonly Mock<ISystemReviewRepository> _systemReviewRepoMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly CreateReviewCommandHandler _handler;

    public CreateReviewCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _reviewRepoMock = new Mock<IReviewRepository>();
        _systemReviewRepoMock = new Mock<ISystemReviewRepository>();
        _notificationServiceMock = new Mock<INotificationService>();

        _unitOfWorkMock.Setup(u => u.Appointments).Returns(_appointmentRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Reviews).Returns(_reviewRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SystemReviews).Returns(_systemReviewRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _reviewRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _systemReviewRepoMock
            .Setup(r => r.AddAsync(It.IsAny<SystemReview>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _notificationServiceMock
            .Setup(n => n.SendNotificationAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Wakiliy.Domain.Enums.NotificationType>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _handler = new CreateReviewCommandHandler(_unitOfWorkMock.Object, _notificationServiceMock.Object);
    }

    // ─────────────────────────────────────────────
    //  Appointment not found
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenAppointmentNotFound_ReturnsAppointmentNotFoundFailure()
    {
        // Arrange
        var command = BuildValidCommand();
        _appointmentRepoMock
            .Setup(r => r.GetByIdAsync(command.AppointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.AppointmentNotFound);

        _reviewRepoMock.Verify(r =>
            r.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ─────────────────────────────────────────────
    //  Appointment not completed
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(AppointmentStatus.Pending)]
    [InlineData(AppointmentStatus.Confirmed)]
    [InlineData(AppointmentStatus.Cancelled)]
    public async Task Handle_WhenAppointmentIsNotCompleted_ReturnsAppointmentNotCompletedFailure(
        AppointmentStatus status)
    {
        // Arrange
        var command = BuildValidCommand();
        var appointment = CreateAppointment(command.AppointmentId, status);

        _appointmentRepoMock
            .Setup(r => r.GetByIdAsync(command.AppointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.AppointmentNotCompleted);
    }

    // ─────────────────────────────────────────────
    //  Review already exists
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenReviewAlreadyExists_ReturnsReviewAlreadyExistsFailure()
    {
        // Arrange
        var command = BuildValidCommand();
        var appointment = CreateAppointment(command.AppointmentId, AppointmentStatus.Completed);

        _appointmentRepoMock
            .Setup(r => r.GetByIdAsync(command.AppointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        _reviewRepoMock
            .Setup(r => r.ExistsByAppointmentIdAsync(command.AppointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.ReviewAlreadyExists);
    }

    // ─────────────────────────────────────────────
    //  Success — no system review
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithValidRequestAndNoSystemReview_CreatesReviewAndReturnsSuccess()
    {
        // Arrange
        var command = BuildValidCommand(systemReview: null);
        var appointment = CreateAppointment(command.AppointmentId, AppointmentStatus.Completed);

        SetupValidPreconditions(command, appointment, isFirstReview: false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _reviewRepoMock.Verify(r =>
            r.AddAsync(It.Is<Review>(rv =>
                rv.UserId == command.UserId &&
                rv.AppointmentId == command.AppointmentId &&
                rv.Rating == command.LawyerReview.Rating),
            It.IsAny<CancellationToken>()),
            Times.Once);

        _systemReviewRepoMock.Verify(r =>
            r.AddAsync(It.IsAny<SystemReview>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(u =>
            u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ─────────────────────────────────────────────
    //  Success — is first review + system review provided
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenIsFirstReviewAndSystemReviewProvided_AddsSystemReviewToo()
    {
        // Arrange
        var command = BuildValidCommand(systemReview: BuildSystemReviewDto());
        var appointment = CreateAppointment(command.AppointmentId, AppointmentStatus.Completed);

        SetupValidPreconditions(command, appointment, isFirstReview: true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _reviewRepoMock.Verify(r =>
            r.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _systemReviewRepoMock.Verify(r =>
            r.AddAsync(It.Is<SystemReview>(sr =>
                sr.UserId == command.UserId &&
                sr.Rating == command.SystemReview!.Rating),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ─────────────────────────────────────────────
    //  Not first review — system review NOT saved even if provided
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenNotFirstReviewAndSystemReviewProvided_DoesNotAddSystemReview()
    {
        // Arrange
        var command = BuildValidCommand(systemReview: BuildSystemReviewDto());
        var appointment = CreateAppointment(command.AppointmentId, AppointmentStatus.Completed);

        SetupValidPreconditions(command, appointment, isFirstReview: false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _systemReviewRepoMock.Verify(r =>
            r.AddAsync(It.IsAny<SystemReview>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ─────────────────────────────────────────────
    //  Is first review but no system review provided — skip system review
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenIsFirstReviewButNoSystemReviewProvided_DoesNotAddSystemReview()
    {
        // Arrange
        var command = BuildValidCommand(systemReview: null);
        var appointment = CreateAppointment(command.AppointmentId, AppointmentStatus.Completed);

        SetupValidPreconditions(command, appointment, isFirstReview: true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _systemReviewRepoMock.Verify(r =>
            r.AddAsync(It.IsAny<SystemReview>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ─────────────────────────────────────────────
    //  Review entity has correct AI Analysis
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenSuccessful_ReviewHasCorrectAiAnalysis()
    {
        // Arrange
        var command = BuildValidCommand();
        command.LawyerReview.AiReview = new AiReviewDto
        {
            IsFlagged = true,
            Confidence = 0.95,
            Summary = "Suspicious review"
        };
        var appointment = CreateAppointment(command.AppointmentId, AppointmentStatus.Completed);
        SetupValidPreconditions(command, appointment, isFirstReview: false);

        Review? capturedReview = null;
        _reviewRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()))
            .Callback<Review, CancellationToken>((r, _) => capturedReview = r)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedReview.Should().NotBeNull();
        capturedReview!.AiAnalysis.IsFlagged.Should().BeTrue();
        capturedReview.AiAnalysis.Confidence.Should().Be(0.95);
        capturedReview.AiAnalysis.Summary.Should().Be("Suspicious review");
    }

    // ─────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────

    private static CreateReviewCommand BuildValidCommand(SystemReviewDto? systemReview = null)
    {
        return new CreateReviewCommand
        {
            UserId = "user-id-abc",
            AppointmentId = Guid.NewGuid(),
            LawyerReview = new LawyerReviewDto
            {
                Rating = 4.5,
                Comment = "Excellent service and professional advice.",
                AiReview = new AiReviewDto { IsFlagged = false, Confidence = 0.1, Summary = "Positive" }
            },
            SystemReview = systemReview
        };
    }

    private static SystemReviewDto BuildSystemReviewDto() =>
        new()
        {
            Rating = 4.0,
            Comment = "Great platform overall.",
            AiReview = new AiReviewDto { IsFlagged = false, Confidence = 0.05, Summary = "Positive" }
        };

    private static Appointment CreateAppointment(Guid id, AppointmentStatus status) =>
        new()
        {
            Id = id,
            LawyerId = "lawyer-id-xyz",
            ClientId = "client-id-abc",
            Status = status
        };

    private void SetupValidPreconditions(
        CreateReviewCommand command,
        Appointment appointment,
        bool isFirstReview)
    {
        _appointmentRepoMock
            .Setup(r => r.GetByIdAsync(command.AppointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        _reviewRepoMock
            .Setup(r => r.ExistsByAppointmentIdAsync(command.AppointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _systemReviewRepoMock
            .Setup(r => r.IsFirstReviewForUserAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(isFirstReview);
    }
}

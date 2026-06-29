using FluentAssertions;
using Hangfire;
using Moq;
using Wakiliy.Application.Features.Reviews.Commands.Create;
using Wakiliy.Application.BackgroundJobs;
using Wakiliy.Application.Interfaces.Services;
using Wakiliy.Application.Features.Reviews.DTOs;
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
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IBackgroundJobClient> _backgroundJobClientMock;
    private readonly CreateReviewCommandHandler _handler;

    public CreateReviewCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _reviewRepoMock = new Mock<IReviewRepository>();
        _notificationServiceMock = new Mock<INotificationService>();
        _backgroundJobClientMock = new Mock<IBackgroundJobClient>();

        _unitOfWorkMock.Setup(u => u.Appointments).Returns(_appointmentRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Reviews).Returns(_reviewRepoMock.Object);

        _handler = new CreateReviewCommandHandler(
            _unitOfWorkMock.Object,
            _notificationServiceMock.Object,
            _backgroundJobClientMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAppointmentNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new CreateReviewCommand { AppointmentId = Guid.NewGuid(), UserId = "user-1", LawyerReview = new LawyerReviewDto { Rating = 5, Comment = "Great" } };
        _appointmentRepoMock.Setup(r => r.GetByIdAsync(command.AppointmentId, default)).ReturnsAsync((Appointment)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.AppointmentNotFound);
    }

    [Fact]
    public async Task Handle_WhenAppointmentNotCompleted_ReturnsFailure()
    {
        // Arrange
        var appointment = new Appointment { Id = Guid.NewGuid(), Status = AppointmentStatus.Pending };
        var command = new CreateReviewCommand { AppointmentId = appointment.Id, UserId = "user-1", LawyerReview = new LawyerReviewDto { Rating = 5, Comment = "Great" } };
        _appointmentRepoMock.Setup(r => r.GetByIdAsync(command.AppointmentId, default)).ReturnsAsync(appointment);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.AppointmentNotCompleted);
    }

    [Fact]
    public async Task Handle_WhenReviewAlreadyExists_ReturnsFailure()
    {
        // Arrange
        var appointment = new Appointment { Id = Guid.NewGuid(), Status = AppointmentStatus.Completed };
        var command = new CreateReviewCommand { AppointmentId = appointment.Id, UserId = "user-1", LawyerReview = new LawyerReviewDto { Rating = 5, Comment = "Great" } };
        
        _appointmentRepoMock.Setup(r => r.GetByIdAsync(command.AppointmentId, default)).ReturnsAsync(appointment);
        _reviewRepoMock.Setup(r => r.ExistsByAppointmentIdAsync(command.AppointmentId, default)).ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.ReviewAlreadyExists);
    }

    [Fact]
    public async Task Handle_WithValidCommand_CreatesReviewAndReturnsSuccess()
    {
        // Arrange
        var appointment = new Appointment { Id = Guid.NewGuid(), Status = AppointmentStatus.Completed, LawyerId = "lawyer-1" };
        var command = new CreateReviewCommand { AppointmentId = appointment.Id, UserId = "user-1", LawyerReview = new LawyerReviewDto { Rating = 5, Comment = "Great" } };
        
        _appointmentRepoMock.Setup(r => r.GetByIdAsync(command.AppointmentId, default)).ReturnsAsync(appointment);
        _reviewRepoMock.Setup(r => r.ExistsByAppointmentIdAsync(command.AppointmentId, default)).ReturnsAsync(false);
        _reviewRepoMock.Setup(r => r.AddAsync(It.IsAny<Review>(), default)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Rating.Should().Be(command.LawyerReview.Rating);
        result.Value.Comment.Should().Be(command.LawyerReview.Comment);

        _reviewRepoMock.Verify(r => r.AddAsync(It.Is<Review>(rv => rv.AppointmentId == command.AppointmentId && rv.Rating == 5), default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        
        _backgroundJobClientMock.Verify(x => x.Create(
            It.Is<Hangfire.Common.Job>(job => job.Type == typeof(ReviewModerationJob) && job.Method.Name == nameof(ReviewModerationJob.ProcessReviewAsync)),
            It.IsAny<Hangfire.States.EnqueuedState>()), 
            Times.Once);

        _notificationServiceMock.Verify(n => n.SendNotificationAsync(
            appointment.LawyerId,
            "تقييم جديد",
            "حصلت على تقييم جديد من أحد عملائك.",
            NotificationType.NewReview,
            It.IsAny<string>(),
            default), Times.Once);
    }
}

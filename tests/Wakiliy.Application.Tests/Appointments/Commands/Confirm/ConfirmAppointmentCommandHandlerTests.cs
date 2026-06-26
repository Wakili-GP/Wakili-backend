using FluentAssertions;
using Moq;
using Wakiliy.Application.Features.Appointments.Commands.Confirm;
using Wakiliy.Application.Interfaces.Services;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;

namespace Wakiliy.Application.Tests.Appointments.Commands.Confirm;

public class ConfirmAppointmentCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly ConfirmAppointmentCommandHandler _handler;

    public ConfirmAppointmentCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _notificationServiceMock = new Mock<INotificationService>();

        _unitOfWorkMock.Setup(u => u.Appointments).Returns(_appointmentRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _notificationServiceMock
            .Setup(n => n.SendNotificationAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Wakiliy.Domain.Enums.NotificationType>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _handler = new ConfirmAppointmentCommandHandler(_unitOfWorkMock.Object, _notificationServiceMock.Object);
    }

    // ─────────────────────────────────────────────
    //  Appointment not found
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenAppointmentNotFound_ReturnsAppointmentNotFoundFailure()
    {
        // Arrange
        var command = BuildCommand();
        _appointmentRepoMock
            .Setup(r => r.GetByIdAsync(command.AppointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AppointmentErrors.AppointmentNotFound);

        _appointmentRepoMock.Verify(r =>
            r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ─────────────────────────────────────────────
    //  Lawyer ID mismatch (unauthorized)
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenLawyerIdDoesNotMatchAppointment_ReturnsUnauthorizedFailure()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            Id = appointmentId,
            LawyerId = "lawyer-A",
            Status = AppointmentStatus.Pending
        };
        var command = BuildCommand(appointmentId: appointmentId, lawyerId: "lawyer-B");

        _appointmentRepoMock
            .Setup(r => r.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AppointmentErrors.Unauthorized);
    }

    // ─────────────────────────────────────────────
    //  Invalid status transition
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(AppointmentStatus.Confirmed)]
    [InlineData(AppointmentStatus.Completed)]
    [InlineData(AppointmentStatus.Cancelled)]
    public async Task Handle_WhenAppointmentStatusIsNotPending_ReturnsInvalidStatusTransitionFailure(
        AppointmentStatus currentStatus)
    {
        // Arrange
        var lawyerId = "lawyer-id";
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            Id = appointmentId,
            LawyerId = lawyerId,
            Status = currentStatus
        };
        var command = BuildCommand(appointmentId: appointmentId, lawyerId: lawyerId);

        _appointmentRepoMock
            .Setup(r => r.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AppointmentErrors.InvalidStatusTransition);
    }

    // ─────────────────────────────────────────────
    //  Success
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithValidRequest_ConfirmsAppointmentAndReturnsSuccess()
    {
        // Arrange
        var lawyerId = "lawyer-id";
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            Id = appointmentId,
            LawyerId = lawyerId,
            Status = AppointmentStatus.Pending
        };
        var command = BuildCommand(appointmentId: appointmentId, lawyerId: lawyerId);

        _appointmentRepoMock
            .Setup(r => r.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        _appointmentRepoMock
            .Setup(r => r.UpdateAsync(appointment, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        appointment.Status.Should().Be(AppointmentStatus.Confirmed);
        appointment.ConfirmedAt.Should().NotBeNull();
        appointment.ConfirmedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _appointmentRepoMock.Verify(r =>
            r.UpdateAsync(appointment, It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(u =>
            u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ─────────────────────────────────────────────
    //  Helper
    // ─────────────────────────────────────────────

    private static ConfirmAppointmentCommand BuildCommand(
        Guid? appointmentId = null,
        string lawyerId = "lawyer-id")
    {
        return new ConfirmAppointmentCommand
        {
            AppointmentId = appointmentId ?? Guid.NewGuid(),
            LawyerId = lawyerId
        };
    }
}

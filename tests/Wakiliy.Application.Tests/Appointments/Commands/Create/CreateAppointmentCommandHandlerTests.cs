using FluentAssertions;
using Moq;
using Wakiliy.Application.Features.Appointments.Commands.Create;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;

namespace Wakiliy.Application.Tests.Appointments.Commands.Create;

public class CreateAppointmentCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILawyerRepository> _lawyerRepoMock;
    private readonly Mock<IAppointmentSlotRepository> _slotRepoMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly CreateAppointmentCommandHandler _handler;

    public CreateAppointmentCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _lawyerRepoMock = new Mock<ILawyerRepository>();
        _slotRepoMock = new Mock<IAppointmentSlotRepository>();
        _appointmentRepoMock = new Mock<IAppointmentRepository>();

        _unitOfWorkMock.Setup(u => u.Lawyers).Returns(_lawyerRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.AppointmentSlots).Returns(_slotRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Appointments).Returns(_appointmentRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _handler = new CreateAppointmentCommandHandler(_unitOfWorkMock.Object);
    }

    // ─────────────────────────────────────────────
    //  Lawyer not found
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenLawyerNotFound_ReturnsLawyerNotFoundFailure()
    {
        // Arrange
        var command = BuildValidCommand();
        _lawyerRepoMock
            .Setup(r => r.GetByIdAsync(command.LawyerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Lawyer?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AppointmentErrors.LawyerNotFound);

        _appointmentRepoMock.Verify(r =>
            r.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ─────────────────────────────────────────────
    //  Client booking own slot
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenClientIdEqualsLawyerId_ReturnsCannotBookOwnSlotFailure()
    {
        // Arrange
        var lawyerId = "lawyer-id-123";
        var command = BuildValidCommand(lawyerId: lawyerId, clientId: lawyerId); // same ID

        var lawyer = new Lawyer { Id = lawyerId };
        _lawyerRepoMock
            .Setup(r => r.GetByIdAsync(lawyerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lawyer);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AppointmentErrors.CannotBookOwnSlot);
    }

    // ─────────────────────────────────────────────
    //  Slot not found
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenSlotNotFound_ReturnsSlotNotFoundFailure()
    {
        // Arrange
        var command = BuildValidCommand();
        SetupLawyerFound(command.LawyerId);

        _slotRepoMock
            .Setup(r => r.GetByIdAsync(command.SlotId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppointmentSlot?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AppointmentErrors.SlotNotFound);
    }

    // ─────────────────────────────────────────────
    //  Slot already booked
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenSlotIsAlreadyBooked_ReturnsSlotAlreadyBookedFailure()
    {
        // Arrange
        var command = BuildValidCommand();
        SetupLawyerFound(command.LawyerId);
        SetupSlotFound(command.SlotId);

        _appointmentRepoMock
            .Setup(r => r.IsSlotBookedAsync(command.SlotId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AppointmentErrors.SlotAlreadyBooked);
    }

    // ─────────────────────────────────────────────
    //  Success
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithValidRequest_CreatesAppointmentAndReturnsSuccess()
    {
        // Arrange
        var command = BuildValidCommand();
        SetupLawyerFound(command.LawyerId);
        SetupSlotFound(command.SlotId);

        _appointmentRepoMock
            .Setup(r => r.IsSlotBookedAsync(command.SlotId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Appointment? capturedAppointment = null;
        _appointmentRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .Callback<Appointment, CancellationToken>((a, _) => capturedAppointment = a)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        capturedAppointment.Should().NotBeNull();
        capturedAppointment!.SlotId.Should().Be(command.SlotId);
        capturedAppointment.ClientId.Should().Be(command.ClientId);
        capturedAppointment.LawyerId.Should().Be(command.LawyerId);
        capturedAppointment.Status.Should().Be(Domain.Enums.AppointmentStatus.Pending);

        _appointmentRepoMock.Verify(r =>
            r.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(u =>
            u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ─────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────

    private static CreateAppointmentCommand BuildValidCommand(
        string? lawyerId = null,
        string? clientId = null,
        int slotId = 1)
    {
        return new CreateAppointmentCommand
        {
            LawyerId = lawyerId ?? "lawyer-id-abc",
            ClientId = clientId ?? "client-id-xyz",
            SlotId = slotId
        };
    }

    private void SetupLawyerFound(string lawyerId)
    {
        _lawyerRepoMock
            .Setup(r => r.GetByIdAsync(lawyerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Lawyer { Id = lawyerId });
    }

    private void SetupSlotFound(int slotId)
    {
        _slotRepoMock
            .Setup(r => r.GetByIdAsync(slotId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppointmentSlot { Id = slotId });
    }
}

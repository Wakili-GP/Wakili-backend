using FluentAssertions;
using Moq;
using Wakiliy.Application.Features.Appointments.Queries.GetByClient;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Repositories;

namespace Wakiliy.Application.Tests.Appointments.Queries.GetByClient;

public class GetAppointmentsByClientQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly GetAppointmentsByClientQueryHandler _handler;

    public GetAppointmentsByClientQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _appointmentRepoMock = new Mock<IAppointmentRepository>();

        _unitOfWorkMock.Setup(u => u.Appointments).Returns(_appointmentRepoMock.Object);

        _handler = new GetAppointmentsByClientQueryHandler(_unitOfWorkMock.Object);
    }

    // ─────────────────────────────────────────────
    //  Returns populated list
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenClientHasAppointments_ReturnsSuccessWithAppointmentList()
    {
        // Arrange
        var clientId = "client-id-abc";
        var appointments = new List<Appointment>
        {
            new() { Id = Guid.NewGuid(), ClientId = clientId, Status = AppointmentStatus.Pending },
            new() { Id = Guid.NewGuid(), ClientId = clientId, Status = AppointmentStatus.Confirmed }
        };

        _appointmentRepoMock
            .Setup(r => r.GetByClientIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        var query = new GetAppointmentsByClientQuery { ClientId = clientId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);

        _appointmentRepoMock.Verify(r =>
            r.GetByClientIdAsync(clientId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ─────────────────────────────────────────────
    //  Empty list
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenClientHasNoAppointments_ReturnsSuccessWithEmptyList()
    {
        // Arrange
        var clientId = "client-no-appointments";
        _appointmentRepoMock
            .Setup(r => r.GetByClientIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Appointment>());

        var query = new GetAppointmentsByClientQuery { ClientId = clientId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────
    //  Correct client ID forwarded to repository
    // ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_AlwaysForwardsClientIdToRepository()
    {
        // Arrange
        var clientId = "specific-client-id";
        _appointmentRepoMock
            .Setup(r => r.GetByClientIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Appointment>());

        var query = new GetAppointmentsByClientQuery { ClientId = clientId };

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert — ensure the correct clientId was passed
        _appointmentRepoMock.Verify(r =>
            r.GetByClientIdAsync(clientId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

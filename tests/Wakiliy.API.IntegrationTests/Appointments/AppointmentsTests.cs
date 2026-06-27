using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Wakiliy.API.IntegrationTests.Authentication.Helpers;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Xunit;

namespace Wakiliy.API.IntegrationTests.Appointments;

[Collection("IntegrationTests")]
public class AppointmentsTests : BaseIntegrationTest
{
    public AppointmentsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetMyAppointments_ShouldReturnOk_WithClientAppointments()
    {
        // Arrange
        var clientEmail = "client.getmyappointments@test.com";
        var clientHttp = await AuthHelper.AuthenticateUserAsync(Factory, clientEmail, "Password123!", "Client");
        var clientUser = await DbContext.Users.OfType<Client>().FirstAsync(c => c.Email == clientEmail);

        var lawyerEmail = "lawyer.forclientappointments@test.com";
        await AuthHelper.AuthenticateUserAsync(Factory, lawyerEmail, "Password123!", "Lawyer");
        var lawyerUser = await DbContext.Users.OfType<Lawyer>().FirstAsync(l => l.Email == lawyerEmail);

        var slot = new AppointmentSlot
        {
            LawyerId = lawyerUser.Id,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(11, 0, 0),
            SessionType = SessionType.Phone,
            CreatedById = lawyerUser.Id
        };
        DbContext.AppointmentSlots.Add(slot);
        await DbContext.SaveChangesAsync();

        var appointment = new Appointment
        {
            ClientId = clientUser.Id,
            LawyerId = lawyerUser.Id,
            SlotId = slot.Id,
            Status = AppointmentStatus.Pending
        };
        DbContext.Appointments.Add(appointment);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await clientHttp.GetAsync("/api/Appointments/my");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetReceivedAppointments_ShouldReturnOk_WithLawyerAppointments()
    {
        // Arrange
        var lawyerEmail = "lawyer.getreceived@test.com";
        var lawyerHttp = await AuthHelper.AuthenticateUserAsync(Factory, lawyerEmail, "Password123!", "Lawyer");
        var lawyerUser = await DbContext.Users.OfType<Lawyer>().FirstAsync(l => l.Email == lawyerEmail);

        var clientEmail = "client.forreceived@test.com";
        await AuthHelper.AuthenticateUserAsync(Factory, clientEmail, "Password123!", "Client");
        var clientUser = await DbContext.Users.OfType<Client>().FirstAsync(c => c.Email == clientEmail);

        var slot = new AppointmentSlot
        {
            LawyerId = lawyerUser.Id,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            StartTime = new TimeSpan(11, 0, 0),
            EndTime = new TimeSpan(12, 0, 0),
            SessionType = SessionType.InOffice,
            CreatedById = lawyerUser.Id
        };
        DbContext.AppointmentSlots.Add(slot);
        await DbContext.SaveChangesAsync();

        var appointment = new Appointment
        {
            ClientId = clientUser.Id,
            LawyerId = lawyerUser.Id,
            SlotId = slot.Id,
            Status = AppointmentStatus.Pending
        };
        DbContext.Appointments.Add(appointment);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await lawyerHttp.GetAsync("/api/Appointments/received");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Confirm_ShouldReturnOk_WhenAppointmentIsPending()
    {
        // Arrange
        var lawyerEmail = "lawyer.confirm@test.com";
        var lawyerHttp = await AuthHelper.AuthenticateUserAsync(Factory, lawyerEmail, "Password123!", "Lawyer");
        var lawyerUser = await DbContext.Users.OfType<Lawyer>().FirstAsync(l => l.Email == lawyerEmail);

        var clientEmail = "client.confirm@test.com";
        await AuthHelper.AuthenticateUserAsync(Factory, clientEmail, "Password123!", "Client");
        var clientUser = await DbContext.Users.OfType<Client>().FirstAsync(c => c.Email == clientEmail);

        var slot = new AppointmentSlot
        {
            LawyerId = lawyerUser.Id,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            StartTime = new TimeSpan(12, 0, 0),
            EndTime = new TimeSpan(13, 0, 0),
            SessionType = SessionType.Phone,
            CreatedById = lawyerUser.Id
        };
        DbContext.AppointmentSlots.Add(slot);
        await DbContext.SaveChangesAsync();

        var appointment = new Appointment
        {
            ClientId = clientUser.Id,
            LawyerId = lawyerUser.Id,
            SlotId = slot.Id,
            Status = AppointmentStatus.Pending
        };
        DbContext.Appointments.Add(appointment);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await lawyerHttp.PutAsync($"/api/Appointments/{appointment.Id}/confirm", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await DbContext.Entry(appointment).ReloadAsync();
        appointment.Status.Should().Be(AppointmentStatus.Confirmed);
    }

    [Fact]
    public async Task Reject_ShouldReturnOk_WhenAppointmentIsPending()
    {
        // Arrange
        var lawyerEmail = "lawyer.reject@test.com";
        var lawyerHttp = await AuthHelper.AuthenticateUserAsync(Factory, lawyerEmail, "Password123!", "Lawyer");
        var lawyerUser = await DbContext.Users.OfType<Lawyer>().FirstAsync(l => l.Email == lawyerEmail);

        var clientEmail = "client.reject@test.com";
        await AuthHelper.AuthenticateUserAsync(Factory, clientEmail, "Password123!", "Client");
        var clientUser = await DbContext.Users.OfType<Client>().FirstAsync(c => c.Email == clientEmail);

        var slot = new AppointmentSlot
        {
            LawyerId = lawyerUser.Id,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4)),
            StartTime = new TimeSpan(13, 0, 0),
            EndTime = new TimeSpan(14, 0, 0),
            SessionType = SessionType.Phone,
            CreatedById = lawyerUser.Id
        };
        DbContext.AppointmentSlots.Add(slot);
        await DbContext.SaveChangesAsync();

        var appointment = new Appointment
        {
            ClientId = clientUser.Id,
            LawyerId = lawyerUser.Id,
            SlotId = slot.Id,
            Status = AppointmentStatus.Pending
        };
        DbContext.Appointments.Add(appointment);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await lawyerHttp.PutAsync($"/api/Appointments/{appointment.Id}/reject", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await DbContext.Entry(appointment).ReloadAsync();
        appointment.Status.Should().Be(AppointmentStatus.Cancelled);
    }

    [Fact]
    public async Task Complete_ShouldReturnOk_WhenAppointmentIsConfirmed()
    {
        // Arrange
        var lawyerEmail = "lawyer.complete@test.com";
        var lawyerHttp = await AuthHelper.AuthenticateUserAsync(Factory, lawyerEmail, "Password123!", "Lawyer");
        var lawyerUser = await DbContext.Users.OfType<Lawyer>().FirstAsync(l => l.Email == lawyerEmail);

        var clientEmail = "client.complete@test.com";
        await AuthHelper.AuthenticateUserAsync(Factory, clientEmail, "Password123!", "Client");
        var clientUser = await DbContext.Users.OfType<Client>().FirstAsync(c => c.Email == clientEmail);

        var slot = new AppointmentSlot
        {
            LawyerId = lawyerUser.Id,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), // In the past
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(15, 0, 0),
            SessionType = SessionType.Phone,
            CreatedById = lawyerUser.Id
        };
        DbContext.AppointmentSlots.Add(slot);
        await DbContext.SaveChangesAsync();

        var appointment = new Appointment
        {
            ClientId = clientUser.Id,
            LawyerId = lawyerUser.Id,
            SlotId = slot.Id,
            Status = AppointmentStatus.Confirmed,
        };
        DbContext.Appointments.Add(appointment);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await lawyerHttp.PutAsync($"/api/Appointments/{appointment.Id}/complete", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await DbContext.Entry(appointment).ReloadAsync();
        appointment.Status.Should().Be(AppointmentStatus.Completed);
    }

    [Fact]
    public async Task GetApprovedAppointments_ShouldReturnOk_ForLawyer()
    {
        // Arrange
        var lawyerEmail = "lawyer.approved@test.com";
        var lawyerHttp = await AuthHelper.AuthenticateUserAsync(Factory, lawyerEmail, "Password123!", "Lawyer");
        var lawyerUser = await DbContext.Users.OfType<Lawyer>().FirstAsync(l => l.Email == lawyerEmail);

        var clientEmail = "client.approved@test.com";
        await AuthHelper.AuthenticateUserAsync(Factory, clientEmail, "Password123!", "Client");
        var clientUser = await DbContext.Users.OfType<Client>().FirstAsync(c => c.Email == clientEmail);

        var slot = new AppointmentSlot
        {
            LawyerId = lawyerUser.Id,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(11, 0, 0),
            SessionType = SessionType.Phone,
            CreatedById = lawyerUser.Id
        };
        DbContext.AppointmentSlots.Add(slot);
        await DbContext.SaveChangesAsync();

        var appointment = new Appointment
        {
            ClientId = clientUser.Id,
            LawyerId = lawyerUser.Id,
            SlotId = slot.Id,
            Status = AppointmentStatus.Confirmed,
        };
        DbContext.Appointments.Add(appointment);
        await DbContext.SaveChangesAsync();

        // Act
        // CalendarViewType.Month = 1
        var response = await lawyerHttp.GetAsync("/api/Appointments/approved?viewType=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

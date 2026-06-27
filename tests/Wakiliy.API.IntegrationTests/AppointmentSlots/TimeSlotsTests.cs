using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Wakiliy.API.IntegrationTests.Authentication.Helpers;
using Wakiliy.Application.Features.AppointmentSlots.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Xunit;

namespace Wakiliy.API.IntegrationTests.AppointmentSlots;

[Collection("IntegrationTests")]
public class TimeSlotsTests : BaseIntegrationTest
{
    public TimeSlotsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenDataIsValid()
    {
        // Arrange
        var email = "lawyer.createslot@test.com";
        var client = await AuthHelper.AuthenticateUserAsync(Factory, email, "Password123!", "Lawyer");

        var payload = new CreateAppointmentSlotDto
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(11, 0, 0),
            SessionType = SessionType.Phone
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/TimeSlots", payload);

        // Assert
        var contentStr = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, contentStr);

        var lawyer = await DbContext.Users.OfType<Lawyer>().FirstAsync(l => l.Email == email);
        var slot = await DbContext.AppointmentSlots.FirstOrDefaultAsync(s => s.LawyerId == lawyer.Id);
        
        slot.Should().NotBeNull();
        slot!.Date.Should().Be(payload.Date);
        slot.StartTime.Should().Be(payload.StartTime);
    }

    [Fact]
    public async Task Create_ShouldReturnConflict_WhenSlotOverlaps()
    {
        // Arrange
        var email = "lawyer.overlapslot@test.com";
        var client = await AuthHelper.AuthenticateUserAsync(Factory, email, "Password123!", "Lawyer");
        var lawyer = await DbContext.Users.OfType<Lawyer>().FirstAsync(l => l.Email == email);

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));
        var slot = new AppointmentSlot
        {
            LawyerId = lawyer.Id,
            Date = date,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(15, 0, 0),
            SessionType = SessionType.InOffice,
            CreatedById = lawyer.Id
        };
        DbContext.AppointmentSlots.Add(slot);
        await DbContext.SaveChangesAsync();

        var payload = new CreateAppointmentSlotDto
        {
            Date = date,
            StartTime = new TimeSpan(14, 30, 0),
            EndTime = new TimeSpan(15, 30, 0), // Overlaps
            SessionType = SessionType.Phone
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/TimeSlots", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetMyTimeSlots_ShouldReturnOk_WithLawyerSlots()
    {
        // Arrange
        var email = "lawyer.getmyslots@test.com";
        var client = await AuthHelper.AuthenticateUserAsync(Factory, email, "Password123!", "Lawyer");
        var lawyer = await DbContext.Users.OfType<Lawyer>().FirstAsync(l => l.Email == email);

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));
        DbContext.AppointmentSlots.Add(new AppointmentSlot
        {
            LawyerId = lawyer.Id,
            Date = date,
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0),
            SessionType = SessionType.Phone,
            CreatedById = lawyer.Id
        });
        await DbContext.SaveChangesAsync();

        // Act
        var response = await client.GetAsync($"/api/TimeSlots?date={date.ToString("yyyy-MM-dd")}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetLawyerTimeSlots_ShouldReturnOk_WithPublicSlots()
    {
        // Arrange
        var lawyerEmail = "lawyer.getpublicslots@test.com";
        await AuthHelper.AuthenticateUserAsync(Factory, lawyerEmail, "Password123!", "Lawyer");
        var lawyer = await DbContext.Users.OfType<Lawyer>().FirstAsync(l => l.Email == lawyerEmail);

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4));
        DbContext.AppointmentSlots.Add(new AppointmentSlot
        {
            LawyerId = lawyer.Id,
            Date = date,
            StartTime = new TimeSpan(11, 0, 0),
            EndTime = new TimeSpan(12, 0, 0),
            SessionType = SessionType.InOffice,
            CreatedById = lawyer.Id
        });
        await DbContext.SaveChangesAsync();

        // Any client can view public slots without auth, or with client auth
        var clientClient = await AuthHelper.AuthenticateUserAsync(Factory, "client.getpublicslots@test.com", "Password123!", "Client");

        // Act
        var response = await clientClient.GetAsync($"/api/TimeSlots/lawyer/{lawyer.Id}?date={date.ToString("yyyy-MM-dd")}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenDataIsValid()
    {
        // Arrange
        var email = "lawyer.updateslot@test.com";
        var client = await AuthHelper.AuthenticateUserAsync(Factory, email, "Password123!", "Lawyer");
        var lawyer = await DbContext.Users.OfType<Lawyer>().FirstAsync(l => l.Email == email);

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var slot = new AppointmentSlot
        {
            LawyerId = lawyer.Id,
            Date = date,
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(9, 0, 0),
            SessionType = SessionType.Phone,
            CreatedById = lawyer.Id
        };
        DbContext.AppointmentSlots.Add(slot);
        await DbContext.SaveChangesAsync();

        var payload = new UpdateAppointmentSlotDto
        {
            Date = date,
            StartTime = new TimeSpan(8, 30, 0), // Updated
            EndTime = new TimeSpan(9, 30, 0),   // Updated
            SessionType = SessionType.InOffice
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/TimeSlots/{slot.Id}", payload);

        // Assert
        var contentStr = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, contentStr);

        await DbContext.Entry(slot).ReloadAsync();
        slot.StartTime.Should().Be(payload.StartTime);
        slot.SessionType.Should().Be(SessionType.InOffice);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenSlotExists()
    {
        // Arrange
        var email = "lawyer.deleteslot@test.com";
        var client = await AuthHelper.AuthenticateUserAsync(Factory, email, "Password123!", "Lawyer");
        var lawyer = await DbContext.Users.OfType<Lawyer>().FirstAsync(l => l.Email == email);

        var slot = new AppointmentSlot
        {
            LawyerId = lawyer.Id,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(6)),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(11, 0, 0),
            SessionType = SessionType.Phone,
            CreatedById = lawyer.Id
        };
        DbContext.AppointmentSlots.Add(slot);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await client.DeleteAsync($"/api/TimeSlots/{slot.Id}");

        // Assert
        var contentStr = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent, contentStr);

        var deletedSlot = await DbContext.AppointmentSlots.AsNoTracking().FirstOrDefaultAsync(s => s.Id == slot.Id);
        deletedSlot.Should().BeNull();
    }
}

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Wakiliy.API.IntegrationTests.Authentication.Helpers;
using Wakiliy.Application.Features.Auth.Commands.ResendConfirmEmail;
using Wakiliy.Domain.Entities;
using Xunit;

namespace Wakiliy.API.IntegrationTests.Authentication;

public class ResendVerificationTests : BaseIntegrationTest
{
    public ResendVerificationTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ResendVerification_ShouldReturnTooManyRequests_WhenResendingTooSoon()
    {
        // Arrange
        var email = "resend.toosoon@example.com";
        await AuthHelper.RegisterUserAsync(Client, email, "Password123!", "Client");

        var command = new ResendConfirmEmailCommand { Email = email };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/resend-verification", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public async Task ResendVerification_ShouldReturnOk_WhenUserIsUnverified()
    {
        // Arrange
        var email = "resend.ok@example.com";
        await AuthHelper.RegisterUserAsync(Client, email, "Password123!", "Client");

        // The registration creates the first OTP, we make it old to bypass the rate limit.
        var otps = await DbContext.Set<EmailOtp>().Where(o => o.Email == email).ToListAsync();
        foreach (var otp in otps)
        {
            otp.ExpireAt = DateTime.UtcNow.AddMinutes(-2);
        }
        await DbContext.SaveChangesAsync();

        var initialOtpCount = otps.Count;

        var command = new ResendConfirmEmailCommand { Email = email };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/resend-verification", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify Database
        var currentOtpCount = await DbContext.Set<EmailOtp>().CountAsync(o => o.Email == email);
        currentOtpCount.Should().BeGreaterThan(initialOtpCount, "A new OTP should be generated and saved");
    }

    [Fact]
    public async Task ResendVerification_ShouldReturnBadRequest_WhenUserAlreadyVerified()
    {
        // Arrange
        var email = "alreadyverified.resend@example.com";
        await AuthHelper.RegisterUserAsync(Client, email, "Password123!", "Client");
        await AuthHelper.VerifyEmailAsync(DbContext, email);

        var command = new ResendConfirmEmailCommand { Email = email };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/resend-verification", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResendVerification_ShouldReturnBadRequest_WhenEmailIsUnknown()
    {
        // Arrange
        var command = new ResendConfirmEmailCommand { Email = "unknown.resend@example.com" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/resend-verification", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

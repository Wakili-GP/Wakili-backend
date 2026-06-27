using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Wakiliy.API.IntegrationTests.Authentication.Helpers;
using Wakiliy.Application.Features.Auth.Commands.ResetPassword;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Xunit;

namespace Wakiliy.API.IntegrationTests.Authentication;

public class ResetPasswordTests : BaseIntegrationTest
{
    public ResetPasswordTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    private string HashOtp(string otp)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(otp));
        return Convert.ToBase64String(bytes);
    }

    private async Task SeedOtpAsync(string email, string code, DateTime expiration, bool isUsed = false)
    {
        var emailOtp = new EmailOtp
        {
            Email = email,
            Code = HashOtp(code),
            ExpireAt = expiration,
            IsUsed = isUsed,
            Purpose = OtpPurpose.PasswordReset
        };

        await DbContext.Set<EmailOtp>().AddAsync(emailOtp);
        await DbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnOk_WhenOtpIsValid()
    {
        // Arrange
        var email = "reset@example.com";
        var oldPassword = "OldPassword123!";
        var newPassword = "NewPassword123!";
        await AuthHelper.RegisterUserAsync(Client, email, oldPassword, "Client");
        await AuthHelper.VerifyEmailAsync(DbContext, email);

        var code = "123456";
        await SeedOtpAsync(email, code, DateTime.UtcNow.AddMinutes(15));

        var command = new ResetPasswordCommand(email, code, newPassword);

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/reset-password", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify we can login with new password
        var newLoginResponse = await AuthHelper.LoginAsync(Client, email, newPassword);
        newLoginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify we cannot login with old password
        var oldLoginResponse = await AuthHelper.LoginAsync(Client, email, oldPassword);
        oldLoginResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnBadRequest_WhenOtpIsInvalid()
    {
        // Arrange
        var email = "invalidotp@example.com";
        await AuthHelper.RegisterUserAsync(Client, email, "Password123!", "Client");

        var command = new ResetPasswordCommand(email, "000000", "NewPassword123!");

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/reset-password", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnBadRequest_WhenOtpIsExpired()
    {
        // Arrange
        var email = "expiredotp@example.com";
        await AuthHelper.RegisterUserAsync(Client, email, "Password123!", "Client");

        var code = "123456";
        await SeedOtpAsync(email, code, DateTime.UtcNow.AddMinutes(-5));

        var command = new ResetPasswordCommand(email, code, "NewPassword123!");

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/reset-password", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnBadRequest_WhenOtpIsAlreadyUsed()
    {
        // Arrange
        var email = "usedotp@example.com";
        await AuthHelper.RegisterUserAsync(Client, email, "Password123!", "Client");

        var code = "123456";
        await SeedOtpAsync(email, code, DateTime.UtcNow.AddMinutes(15), isUsed: true);

        var command = new ResetPasswordCommand(email, code, "NewPassword123!");

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/reset-password", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

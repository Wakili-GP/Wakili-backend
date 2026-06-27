using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Wakiliy.API.IntegrationTests.Authentication.Helpers;
using Wakiliy.Application.Features.Auth.Commands.ConfirmEmail;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Xunit;

namespace Wakiliy.API.IntegrationTests.Authentication;

public class VerifyEmailTests : BaseIntegrationTest
{
    public VerifyEmailTests(CustomWebApplicationFactory factory) : base(factory)
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
            Purpose = OtpPurpose.EmailVerification
        };

        await DbContext.Set<EmailOtp>().AddAsync(emailOtp);
        await DbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task VerifyEmail_ShouldReturnOk_WhenCodeIsValid()
    {
        // Arrange
        var email = "verify@example.com";
        await AuthHelper.RegisterUserAsync(Client, email, "Password123!", "Client");
        
        var code = "123456";
        await SeedOtpAsync(email, code, DateTime.UtcNow.AddMinutes(5));

        var command = new ConfirmEmailCommand { Email = email, Code = code };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/verify-email", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify Database
        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        user!.EmailConfirmed.Should().BeTrue();
        
        var otp = await DbContext.Set<EmailOtp>().AsNoTracking().FirstOrDefaultAsync(o => o.Email == email && o.Code == HashOtp(code));
        otp!.IsUsed.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyEmail_ShouldReturnBadRequest_WhenCodeIsInvalid()
    {
        // Arrange
        var email = "invalidcode@example.com";
        await AuthHelper.RegisterUserAsync(Client, email, "Password123!", "Client");
        
        var command = new ConfirmEmailCommand { Email = email, Code = "000000" }; // Wrong code

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/verify-email", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        user!.EmailConfirmed.Should().BeFalse();
    }

    [Fact]
    public async Task VerifyEmail_ShouldReturnBadRequest_WhenCodeIsExpired()
    {
        // Arrange
        var email = "expired@example.com";
        await AuthHelper.RegisterUserAsync(Client, email, "Password123!", "Client");
        
        var code = "123456";
        await SeedOtpAsync(email, code, DateTime.UtcNow.AddMinutes(-5)); // Expired

        var command = new ConfirmEmailCommand { Email = email, Code = code };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/verify-email", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        user!.EmailConfirmed.Should().BeFalse();
    }

    [Fact]
    public async Task VerifyEmail_ShouldReturnBadRequest_WhenEmailAlreadyVerified()
    {
        // Arrange
        var email = "alreadyverified@example.com";
        await AuthHelper.RegisterUserAsync(Client, email, "Password123!", "Client");
        await AuthHelper.VerifyEmailAsync(DbContext, email); // Verify manually
        
        var code = "123456";
        await SeedOtpAsync(email, code, DateTime.UtcNow.AddMinutes(5));

        var command = new ConfirmEmailCommand { Email = email, Code = code };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/verify-email", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task VerifyEmail_ShouldReturnBadRequest_WhenUserIsUnknown()
    {
        // Arrange
        var command = new ConfirmEmailCommand { Email = "unknownuser@example.com", Code = "123456" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/verify-email", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

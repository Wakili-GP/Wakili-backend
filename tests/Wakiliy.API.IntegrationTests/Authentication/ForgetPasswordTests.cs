using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Wakiliy.API.IntegrationTests.Authentication.Helpers;
using Wakiliy.Application.Features.Auth.Commands.ForgotPassword;
using Wakiliy.Domain.Entities;
using Xunit;

namespace Wakiliy.API.IntegrationTests.Authentication;

public class ForgetPasswordTests : BaseIntegrationTest
{
    public ForgetPasswordTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ForgetPassword_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        var email = "forget@example.com";
        await AuthHelper.RegisterUserAsync(Client, email, "Password123!", "Client");

        var command = new ForgetPasswordCommand { Email = email };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/forget-password", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify Database
        // Note: The EmailOtp generated for password reset has Purpose = PasswordReset (assuming enum value 1 or similar)
        var resetOtpExists = await DbContext.Set<EmailOtp>().AnyAsync(o => o.Email == email && o.Purpose == Wakiliy.Domain.Enums.OtpPurpose.PasswordReset);
        resetOtpExists.Should().BeTrue("A password reset OTP should be generated and stored in the database");
    }

    [Fact]
    public async Task ForgetPassword_ShouldReturnSuccess_WhenEmailIsUnknown()
    {
        // Arrange
        var command = new ForgetPasswordCommand { Email = "unknown.forget@example.com" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/forget-password", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

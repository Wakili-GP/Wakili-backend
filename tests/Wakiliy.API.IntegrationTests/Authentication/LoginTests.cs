using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Wakiliy.API.Common;
using Wakiliy.API.IntegrationTests.Authentication.Helpers;
using Wakiliy.Application.Features.Auth.Commands.Login;
using Wakiliy.Application.Features.Auth.DTOs;
using Wakiliy.Domain.Responses;
using Xunit;

namespace Wakiliy.API.IntegrationTests.Authentication;

public class LoginTests : BaseIntegrationTest
{
    public LoginTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenClientCredentialsAreValid()
    {
        // Arrange
        var email = "client@example.com";
        var password = "StrongPassword123!";
        await AuthHelper.RegisterUserAsync(Client, email, password, "Client");
        await AuthHelper.VerifyEmailAsync(DbContext, email);

        var command = new LoginCommand { Email = email, Password = password };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse<LoginResponse>>();
        
        result.Should().NotBeNull();
        result!.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().NotBeNullOrEmpty();
        result.Data.User.Should().NotBeNull();
        result.Data.User.Email.Should().Be(email);
        result.Data.User.UserType.Should().Be("Client");
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenLawyerCredentialsAreValid()
    {
        // Arrange
        var email = "lawyer@example.com";
        var password = "StrongPassword123!";
        await AuthHelper.RegisterUserAsync(Client, email, password, "Lawyer");
        await AuthHelper.VerifyEmailAsync(DbContext, email);

        var command = new LoginCommand { Email = email, Password = password };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse<LoginResponse>>();
        
        result.Should().NotBeNull();
        result!.Data!.AccessToken.Should().NotBeNullOrEmpty();
        result.Data.User.UserType.Should().Be("Lawyer");
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenPasswordIsIncorrect()
    {
        // Arrange
        var email = "test@example.com";
        var password = "StrongPassword123!";
        await AuthHelper.RegisterUserAsync(Client, email, password, "Client");
        await AuthHelper.VerifyEmailAsync(DbContext, email);

        var command = new LoginCommand { Email = email, Password = "WrongPassword!" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenEmailIsUnknown()
    {
        // Arrange
        var command = new LoginCommand { Email = "unknown@example.com", Password = "Password123!" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ShouldReturnForbidden_WhenEmailIsNotVerified()
    {
        // Arrange
        var email = "unverified@example.com";
        var password = "StrongPassword123!";
        await AuthHelper.RegisterUserAsync(Client, email, password, "Client");
        // Intentionally not verifying email

        var command = new LoginCommand { Email = email, Password = password };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", command);

        // Assert
        // Depending on your API, this could be 403 Forbidden or 400 BadRequest. We will assume 403.
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Login_ShouldReturnForbidden_WhenAccountIsLocked()
    {
        // Arrange
        var email = "locked@example.com";
        var password = "StrongPassword123!";
        await AuthHelper.RegisterUserAsync(Client, email, password, "Client");
        await AuthHelper.VerifyEmailAsync(DbContext, email);

        // Lock the account manually via DbContext
        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        user!.LockoutEnabled = true;
        user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(15);
        await DbContext.SaveChangesAsync();

        var command = new LoginCommand { Email = email, Password = password };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}

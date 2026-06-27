using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Wakiliy.API.Common;
using Wakiliy.API.IntegrationTests.Authentication.Helpers;
using Wakiliy.Application.Features.Auth.Commands.AdminLogin;
using Wakiliy.Application.Features.Auth.DTOs;
using Wakiliy.Domain.Responses;
using Xunit;

namespace Wakiliy.API.IntegrationTests.Authentication;

public class AdminLoginTests : BaseIntegrationTest
{
    public AdminLoginTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task AdminLogin_ShouldReturnOk_WhenAdminCredentialsAreValid()
    {
        // Arrange
        // Admin user is seeded via DbSeeder in BaseIntegrationTest
        var command = new AdminLoginCommand { Email = "admin@test.com", Password = "StrongPassword123!" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/admin-login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse<LoginResponse>>();
        
        result.Should().NotBeNull();
        result!.Data!.AccessToken.Should().NotBeNullOrEmpty();
        result.Data.User.UserType.Should().Be("Admin");
    }

    [Fact]
    public async Task AdminLogin_ShouldReturnOk_WhenSuperAdminCredentialsAreValid()
    {
        // Arrange
        // SuperAdmin user is seeded via DbSeeder in BaseIntegrationTest
        var command = new AdminLoginCommand { Email = "superadmin@test.com", Password = "StrongPassword123!" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/admin-login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse<LoginResponse>>();
        
        result.Should().NotBeNull();
        result!.Data!.AccessToken.Should().NotBeNullOrEmpty();
        result.Data.User.UserType.Should().Be("SuperAdmin");
    }

    [Fact]
    public async Task AdminLogin_ShouldReturnForbidden_WhenClientTriesToLogin()
    {
        // Arrange
        var email = "client.admin.test@example.com";
        var password = "StrongPassword123!";
        await AuthHelper.RegisterUserAsync(Client, email, password, "Client");
        await AuthHelper.VerifyEmailAsync(DbContext, email);

        var command = new AdminLoginCommand { Email = email, Password = password };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/admin-login", command);

        // Assert
        // Client should be forbidden from admin login
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdminLogin_ShouldReturnForbidden_WhenLawyerTriesToLogin()
    {
        // Arrange
        var email = "lawyer.admin.test@example.com";
        var password = "StrongPassword123!";
        await AuthHelper.RegisterUserAsync(Client, email, password, "Lawyer");
        await AuthHelper.VerifyEmailAsync(DbContext, email);

        var command = new AdminLoginCommand { Email = email, Password = password };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/admin-login", command);

        // Assert
        // Lawyer should be forbidden from admin login
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdminLogin_ShouldReturnBadRequest_WhenPasswordIsIncorrect()
    {
        // Arrange
        var command = new AdminLoginCommand { Email = "admin@wakili.com", Password = "WrongPassword!" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/admin-login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdminLogin_ShouldReturnBadRequest_WhenEmailIsUnknown()
    {
        // Arrange
        var command = new AdminLoginCommand { Email = "unknown.admin@test.com", Password = "StrongPassword123!" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/admin-login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

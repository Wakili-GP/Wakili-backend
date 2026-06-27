using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Wakiliy.API.Common;
using Wakiliy.API.IntegrationTests.Authentication.Helpers;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Domain.Responses;
using Xunit;

namespace Wakiliy.API.IntegrationTests.Authentication;

public class GetCurrentUserTests : BaseIntegrationTest
{
    public GetCurrentUserTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetCurrentUser_ShouldReturnOk_WhenAuthenticatedAsClient()
    {
        // Arrange
        var email = "client@test.com";
        var authenticatedClient = await AuthHelper.AuthenticateUserAsync(Factory, email, "StrongPassword123!", "Client");

        // Act
        var response = await authenticatedClient.GetAsync("/api/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse<UserInfoResponse>>();
        
        result.Should().NotBeNull();
        result!.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be(email);
        result.Data.UserType.Should().Be("Client");
        result.Data.FirstName.Should().Be("Client");
        result.Data.LastName.Should().Be("User");
    }

    [Fact]
    public async Task GetCurrentUser_ShouldReturnOk_WhenAuthenticatedAsLawyer()
    {
        // Arrange
        var email = "lawyer@test.com";
        var authenticatedClient = await AuthHelper.AuthenticateUserAsync(Factory, email, "StrongPassword123!", "Lawyer");

        // Act
        var response = await authenticatedClient.GetAsync("/api/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse<UserInfoResponse>>();
        
        result.Should().NotBeNull();
        result!.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be(email);
        result.Data.UserType.Should().Be("Lawyer");
    }

    [Fact]
    public async Task GetCurrentUser_ShouldReturnOk_WhenAuthenticatedAsAdmin()
    {
        // Arrange
        var authenticatedClient = await AuthHelper.AuthenticateAdminAsync(Factory, "admin@test.com", "StrongPassword123!");

        // Act
        var response = await authenticatedClient.GetAsync("/api/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse<UserInfoResponse>>();
        
        result.Should().NotBeNull();
        result!.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be("admin@test.com");
        result.Data.UserType.Should().Be("Admin");
    }

    [Fact]
    public async Task GetCurrentUser_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        // Using unauthenticated Client

        // Act
        var response = await Client.GetAsync("/api/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

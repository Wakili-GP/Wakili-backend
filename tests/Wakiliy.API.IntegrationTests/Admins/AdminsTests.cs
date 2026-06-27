using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Wakiliy.API.IntegrationTests.Authentication.Helpers;
using Wakiliy.API.Common;
using Wakiliy.Application.Features.Admins.DTOs;
using Xunit;
using Wakiliy.Application.Features.Admins.Commands.CreateAdmin;

namespace Wakiliy.API.IntegrationTests.Admins;

[Collection("IntegrationTests")]
public class AdminsTests : BaseIntegrationTest
{
    public AdminsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAdmins_ShouldReturnOk_WhenSuperAdminIsAuthenticated()
    {
        // Arrange
        var superAdminHttp = await AuthHelper.AuthenticateAdminAsync(Factory, "superadmin@test.com", "StrongPassword123!");

        // Act
        var response = await superAdminHttp.GetAsync("/api/Admins");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<SuccessResponse<List<AdminDto>>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAdmins_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var clientHttp = Factory.CreateClient();

        // Act
        var response = await clientHttp.GetAsync("/api/Admins");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateAdmin_ShouldReturnCreated_WhenDataIsValid()
    {
        // Arrange
        var superAdminHttp = await AuthHelper.AuthenticateAdminAsync(Factory, "superadmin@test.com", "StrongPassword123!");
        var command = new CreateAdminCommand
        {
            FirstName = "New",
            LastName = "Admin",
            Email = "new.admin@test.com",
            Password = "Password123!"
        };

        // Act
        var response = await superAdminHttp.PostAsJsonAsync("/api/Admins", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<AdminDto>();
        content.Should().NotBeNull();
        content!.Email.Should().Be("new.admin@test.com");
    }

    [Fact]
    public async Task UpdateAdmin_ShouldReturnOk_WhenDataIsValid()
    {
        // Arrange
        var superAdminHttp = await AuthHelper.AuthenticateAdminAsync(Factory, "superadmin@test.com", "StrongPassword123!");
        
        // Create an admin to update
        var createCmd = new CreateAdminCommand
        {
            FirstName = "Update",
            LastName = "Me",
            Email = "update.admin@test.com",
            Password = "Password123!"
        };
        var createResponse = await superAdminHttp.PostAsJsonAsync("/api/Admins", createCmd);
        var createData = await createResponse.Content.ReadFromJsonAsync<AdminDto>();
        var adminId = createData!.Id;

        var updateReq = new UpdateAdminRequest
        {
            FirstName = "UpdatedName",
            LastName = "UpdatedLast",
            Status = "Active"
        };

        // Act
        var response = await superAdminHttp.PutAsJsonAsync($"/api/Admins/{adminId}", updateReq);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteAdmin_ShouldReturnNoContent_WhenAdminExists()
    {
        // Arrange
        var superAdminHttp = await AuthHelper.AuthenticateAdminAsync(Factory, "superadmin@test.com", "StrongPassword123!");
        
        // Create an admin to delete
        var createCmd = new CreateAdminCommand
        {
            FirstName = "Delete",
            LastName = "Me",
            Email = "delete.admin@test.com",
            Password = "Password123!"
        };
        var createResponse = await superAdminHttp.PostAsJsonAsync("/api/Admins", createCmd);
        var createData = await createResponse.Content.ReadFromJsonAsync<AdminDto>();
        var adminId = createData!.Id;

        // Act
        var response = await superAdminHttp.DeleteAsync($"/api/Admins/{adminId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}

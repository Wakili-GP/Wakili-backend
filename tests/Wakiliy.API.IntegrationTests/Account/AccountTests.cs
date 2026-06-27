using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Wakiliy.API.IntegrationTests.Authentication.Helpers;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.API.Common;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Responses;
using Xunit;

namespace Wakiliy.API.IntegrationTests.Account;

[Collection("IntegrationTests")]
public class AccountTests : BaseIntegrationTest
{
    public AccountTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnOk_WhenCurrentPasswordIsCorrect()
    {
        // Arrange
        var clientEmail = "client.password@test.com";
        var currentPassword = "Password123!";
        var newPassword = "NewPassword123!";
        var clientHttp = await AuthHelper.AuthenticateUserAsync(Factory, clientEmail, currentPassword, "Client");

        var payload = new ChangePasswordDto
        {
            CurrentPassword = currentPassword,
            NewPassword = newPassword
        };

        // Act
        var response = await clientHttp.PostAsJsonAsync("/api/Account/change-password", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify new password works
        var loginResponse = await AuthHelper.LoginAsync(Client, clientEmail, newPassword);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetClientData_ShouldReturnOk_WhenClientIsAuthenticated()
    {
        // Arrange
        var clientEmail = "client.info@test.com";
        var clientHttp = await AuthHelper.AuthenticateUserAsync(Factory, clientEmail, "Password123!", "Client");

        // Act
        var response = await clientHttp.GetAsync("/api/Account/client-info");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<SuccessResponse<ClientDataDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        var data = content.Data;
        data.Should().NotBeNull();
        data!.Email.Should().Be(clientEmail);
    }

    [Fact]
    public async Task GetLawyerData_ShouldReturnOk_WhenLawyerIsAuthenticated()
    {
        // Arrange
        var lawyerEmail = "lawyer.info@test.com";
        var lawyerHttp = await AuthHelper.AuthenticateUserAsync(Factory, lawyerEmail, "Password123!", "Lawyer");

        // Act
        var response = await lawyerHttp.GetAsync("/api/Account/lawyer-info");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<SuccessResponse<LawyerDataDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        var data = content.Data;
        data.Should().NotBeNull();
        data!.Email.Should().Be(lawyerEmail);
    }

    [Fact]
    public async Task UpdateClientInfo_ShouldReturnOk_WhenDataIsValid()
    {
        // Arrange
        var clientEmail = "client.updateinfo@test.com";
        var clientHttp = await AuthHelper.AuthenticateUserAsync(Factory, clientEmail, "Password123!", "Client");

        var content = new MultipartFormDataContent();
        content.Add(new StringContent("John"), "FirstName");
        content.Add(new StringContent("Doe"), "LastName");
        content.Add(new StringContent("1234567890"), "PhoneNumber");

        // Act
        var response = await clientHttp.PutAsync("/api/Account/client-info", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var user = await DbContext.Users.OfType<Client>().FirstAsync(c => c.Email == clientEmail);
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task UpdateLawyerInfo_ShouldReturnOk_WhenDataIsValid()
    {
        // Arrange
        var lawyerEmail = "lawyer.updateinfo@test.com";
        var lawyerHttp = await AuthHelper.AuthenticateUserAsync(Factory, lawyerEmail, "Password123!", "Lawyer");

        var content = new MultipartFormDataContent();
        content.Add(new StringContent("0987654321"), "PhoneNumber");
        content.Add(new StringContent("London"), "City");
        content.Add(new StringContent("UK"), "Country");
        content.Add(new StringContent("Test Bio"), "Bio");
        content.Add(new StringContent("150"), "PhoneSessionPrice");

        // Act
        var response = await lawyerHttp.PutAsync("/api/Account/lawyer-info", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var user = await DbContext.Users.OfType<Lawyer>().FirstAsync(l => l.Email == lawyerEmail);
        user.City.Should().Be("London");
        user.Bio.Should().Be("Test Bio");
        user.PhoneSessionPrice.Should().Be(150);
    }
}

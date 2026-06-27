using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Wakiliy.API.IntegrationTests.Authentication.Helpers;
using Wakiliy.API.IntegrationTests.Infrastructure.Helpers;
using Wakiliy.Application.Features.Lawyers.Commands.Verification.RejectVerification;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Xunit;

namespace Wakiliy.API.IntegrationTests.Lawyers;

public class LawyersTests : BaseIntegrationTest
{
    public LawyersTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetLawyerVerificationRequests_ShouldReturnOk_WhenUserIsAdmin()
    {
        // Arrange
        var adminClient = await AuthHelper.AuthenticateAdminAsync(Factory, "admin@test.com", "StrongPassword123!");

        // Act
        var response = await adminClient.GetAsync("/api/lawyers/lawyer-verification");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetLawyerVerificationRequests_ShouldReturnForbidden_WhenUserIsLawyer()
    {
        // Arrange
        var lawyerClient = await AuthHelper.AuthenticateUserAsync(Factory, "unauthorized.lawyer@test.com", "Password123!", "Lawyer");

        // Act
        var response = await lawyerClient.GetAsync("/api/lawyers/lawyer-verification");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ApproveVerification_ShouldReturnOk_AndSendEmail_WhenLawyerExists()
    {
        // Arrange
        var lawyerEmail = "approve.lawyer@test.com";
        await AuthHelper.RegisterUserAsync(Client, lawyerEmail, "Password123!", "Lawyer");
        await TestHelper.SetupLawyerForReviewAsync(DbContext, lawyerEmail);

        var lawyer = await DbContext.Users.OfType<Lawyer>().FirstOrDefaultAsync(u => u.Email == lawyerEmail);
        lawyer.Should().NotBeNull();

        var adminClient = await AuthHelper.AuthenticateAdminAsync(Factory, "admin@test.com", "StrongPassword123!");

        // Act
        var response = await adminClient.PutAsync($"/api/lawyers/verify/approve/{lawyer!.Id}", null);

        // Assert
        var contentStr = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, contentStr);

        // Verify Database
        await DbContext.Entry(lawyer).ReloadAsync();
        lawyer.VerificationStatus.Should().Be(VerificationStatus.Approved);
    }

    [Fact]
    public async Task RejectVerification_ShouldReturnOk_AndSendEmail_WhenLawyerExists()
    {
        // Arrange
        var lawyerEmail = "reject.lawyer@test.com";
        await AuthHelper.RegisterUserAsync(Client, lawyerEmail, "Password123!", "Lawyer");
        await TestHelper.SetupLawyerForReviewAsync(DbContext, lawyerEmail);

        var lawyer = await DbContext.Users.OfType<Lawyer>().FirstOrDefaultAsync(u => u.Email == lawyerEmail);
        lawyer.Should().NotBeNull();

        var adminClient = await AuthHelper.AuthenticateAdminAsync(Factory, "admin@test.com", "StrongPassword123!");

        var request = new RejectVerificationRequest("Documents are blurry.");

        // Act
        var response = await adminClient.PutAsJsonAsync($"/api/lawyers/verify/reject/{lawyer!.Id}", request);

        // Assert
        var contentStr = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, contentStr);

        // Verify Database
        await DbContext.Entry(lawyer).ReloadAsync();
        lawyer.VerificationStatus.Should().Be(VerificationStatus.Rejected);
    }

    [Fact]
    public async Task GetApprovedLawyers_ShouldReturnOk_AndOnlyApprovedLawyers()
    {
        // Arrange
        // We will create two lawyers, one approved, one pending.
        var pendingEmail = "pending.public@test.com";
        await AuthHelper.RegisterUserAsync(Client, pendingEmail, "Password123!", "Lawyer");
        await TestHelper.SetupLawyerForReviewAsync(DbContext, pendingEmail);

        var approvedEmail = "approved.public@test.com";
        await AuthHelper.RegisterUserAsync(Client, approvedEmail, "Password123!", "Lawyer");
        var approvedLawyer = await DbContext.Users.OfType<Lawyer>().FirstOrDefaultAsync(u => u.Email == approvedEmail);
        approvedLawyer!.VerificationStatus = VerificationStatus.Approved;
        approvedLawyer.IsActive = true;
        await DbContext.SaveChangesAsync();

        var clientUser = await AuthHelper.AuthenticateUserAsync(Factory, "client.public@test.com", "Password123!", "Client");

        // Act
        var response = await clientUser.GetAsync("/api/lawyers/approved");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(approvedLawyer.Id);
        content.Should().NotContain(pendingEmail);
    }

    [Fact]
    public async Task GetMyEarnings_ShouldReturnOk_WhenUserIsLawyer()
    {
        // Arrange
        var lawyerEmail = "earnings.lawyer@test.com";
        var lawyerClient = await AuthHelper.AuthenticateUserAsync(Factory, lawyerEmail, "Password123!", "Lawyer");

        // Act
        var response = await lawyerClient.GetAsync("/api/lawyers/me/earnings");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

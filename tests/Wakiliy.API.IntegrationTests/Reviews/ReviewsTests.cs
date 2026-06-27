using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Wakiliy.API.IntegrationTests.Authentication.Helpers;
using Wakiliy.API.Common;
using Wakiliy.Application.Features.Reviews.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Wakiliy.Infrastructure.Data;
using Wakiliy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Wakiliy.API.IntegrationTests.Reviews;

[Collection("IntegrationTests")]
public class ReviewsTests : BaseIntegrationTest
{
    public ReviewsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAllReviews_ShouldReturnOk_WhenAdminIsAuthenticated()
    {
        // Arrange
        var adminHttp = await AuthHelper.AuthenticateAdminAsync(Factory, "superadmin@test.com", "StrongPassword123!");

        // Act
        var response = await adminHttp.GetAsync("/api/reviews/admin");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<SuccessResponse<List<ReviewResponseDto>>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllReviews_ShouldReturnForbidden_WhenClientIsAuthenticated()
    {
        // Arrange
        var clientEmail = "client.reviews@test.com";
        var clientHttp = await AuthHelper.AuthenticateUserAsync(Factory, clientEmail, "Password123!", "Client");

        // Act
        var response = await clientHttp.GetAsync("/api/reviews/admin");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetLawyerReviewStats_ShouldReturnOk_WhenLawyerExists()
    {
        // Arrange
        var lawyerEmail = "lawyer.stats@test.com";
        var clientHttp = Factory.CreateClient();
        
        // Ensure a lawyer exists
        await AuthHelper.AuthenticateUserAsync(Factory, lawyerEmail, "Password123!", "Lawyer");
        
        // Find lawyer ID
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var lawyer = await dbContext.Users.OfType<Lawyer>().FirstOrDefaultAsync(u => u.Email == lawyerEmail);

        // Act
        var response = await clientHttp.GetAsync($"/api/reviews/lawyer/{lawyer.Id}/stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<SuccessResponse<LawyerReviewStatsDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data.TotalReviews.Should().BeGreaterThanOrEqualTo(0);
    }
}

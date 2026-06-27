using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Wakiliy.API.IntegrationTests.Authentication.Helpers;
using Wakiliy.API.IntegrationTests.Infrastructure.Helpers;
using Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveExperience;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Responses;
using Wakiliy.Domain.Entities;
using Xunit;

namespace Wakiliy.API.IntegrationTests.Lawyers;

public class LawyerOnboardingTests : BaseIntegrationTest
{
    public LawyerOnboardingTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task SaveBasicInfo_ShouldReturnOk_WhenDataIsValid()
    {
        var spec = await DbContext.Specializations.FirstOrDefaultAsync();
        if (spec == null)
        {
            var user = await DbContext.Users.FirstAsync();
            spec = new Specialization { Name = "Test Specialization", Description = "Test", CreatedById = user.Id };
            DbContext.Specializations.Add(spec);
            await DbContext.SaveChangesAsync();
        }

        var email = "onboarding.basicinfo@test.com";
        var client = await AuthHelper.AuthenticateUserAsync(Factory, email, "Password123!", "Lawyer");

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("John"), "FirstName");
        content.Add(new StringContent("Doe"), "LastName");
        content.Add(new StringContent("+1234567890"), "PhoneNumber");
        content.Add(new StringContent("USA"), "Country");
        content.Add(new StringContent("New York"), "City");
        content.Add(new StringContent("Experienced lawyer."), "Bio");
        content.Add(new StringContent("5"), "YearsOfExperience");
        content.Add(new StringContent(spec.Id.ToString()), "PracticeAreas"); // Value matches some ID
        content.Add(new StringContent("1"), "SessionTypes"); // 1 = VideoCall

        var dummyImage = TestHelper.CreateDummyFileContent("dummy image", "image/png");
        content.Add(dummyImage, "ProfileImage", "profile.png");

        // Act
        var response = await client.PostAsync("/api/lawyer/onboarding/basic-info", content);

        // Assert
        var contentStr = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, contentStr);

        var lawyer = await DbContext.Users.OfType<Lawyer>().FirstOrDefaultAsync(u => u.Email == email);
        lawyer.Should().NotBeNull();
        lawyer!.CurrentOnboardingStep.Should().Be(2);
    }

    [Fact]
    public async Task SaveEducation_ShouldReturnOk_WhenDataIsValid()
    {
        // Arrange
        var email = "onboarding.edu@test.com";
        var client = await AuthHelper.AuthenticateUserAsync(Factory, email, "Password123!", "Lawyer");

        // Simulate completing Step 1
        var user = await DbContext.Users.OfType<Lawyer>().FirstOrDefaultAsync(u => u.Email == email);
        user!.CurrentOnboardingStep = 2;
        user.CompletedOnboardingSteps = new List<int> { 1 };
        await DbContext.SaveChangesAsync();

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("Bachelor of Laws"), "AcademicQualifications[0].DegreeType");
        content.Add(new StringContent("Harvard Law School"), "AcademicQualifications[0].UniversityName");
        content.Add(new StringContent("Law"), "AcademicQualifications[0].FieldOfStudy");
        content.Add(new StringContent("2014"), "AcademicQualifications[0].GraduationYear");

        // Act
        var response = await client.PostAsync("/api/lawyer/onboarding/education", content);

        // Assert
        var contentStr = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, contentStr);

        var lawyer = await DbContext.Users.OfType<Lawyer>().FirstOrDefaultAsync(u => u.Email == email);
        lawyer!.CurrentOnboardingStep.Should().Be(2);
    }

    [Fact]
    public async Task SaveExperience_ShouldReturnOk_WhenDataIsValid()
    {
        // Arrange
        var email = "onboarding.exp@test.com";
        var client = await AuthHelper.AuthenticateUserAsync(Factory, email, "Password123!", "Lawyer");

        // Simulate completing Step 2
        var user = await DbContext.Users.OfType<Lawyer>().FirstOrDefaultAsync(u => u.Email == email);
        user!.CurrentOnboardingStep = 3;
        user.CompletedOnboardingSteps = new List<int> { 1, 2 };
        await DbContext.SaveChangesAsync();

        var command = new SaveExperienceCommand
        {
            WorkExperiences = new List<WorkExperienceDto>
            {
                new WorkExperienceDto
                {
                    JobTitle = "Senior Partner",
                    OrganizationName = "Smith & Partners",
                    StartYear = "2015",
                    EndYear = "2022",
                    IsCurrentJob = false,
                    Description = "Managed a team of 10."
                }
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/lawyer/onboarding/experience", command);

        // Assert
        var contentStr = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, contentStr);

        var lawyer = await DbContext.Users.OfType<Lawyer>().FirstOrDefaultAsync(u => u.Email == email);
        lawyer!.CurrentOnboardingStep.Should().Be(3);
    }

    [Fact]
    public async Task SaveVerification_ShouldReturnOk_WhenDataIsValid()
    {
        // Arrange
        var email = "onboarding.ver@test.com";
        var client = await AuthHelper.AuthenticateUserAsync(Factory, email, "Password123!", "Lawyer");

        // Simulate completing Step 3
        var user = await DbContext.Users.OfType<Lawyer>().FirstOrDefaultAsync(u => u.Email == email);
        user!.CurrentOnboardingStep = 4;
        user.CompletedOnboardingSteps = new List<int> { 1, 2, 3 };
        await DbContext.SaveChangesAsync();

        using var content = new MultipartFormDataContent();
        
        var idFront = TestHelper.CreateDummyFileContent("id front", "image/png");
        content.Add(idFront, "NationalIdFront", "idFront.png");

        var idBack = TestHelper.CreateDummyFileContent("id back", "image/png");
        content.Add(idBack, "NationalIdBack", "idBack.png");

        var licenseFile = TestHelper.CreateDummyFileContent("license file", "application/pdf");
        content.Add(licenseFile, "License.LicenseFile", "license.pdf");

        content.Add(new StringContent("L-123456"), "License.LicenseNumber");
        content.Add(new StringContent("New York Bar Association"), "License.IssuingAuthority");
        content.Add(new StringContent("2015"), "License.LicenseYear");

        // Act
        var response = await client.PostAsync("/api/lawyer/onboarding/verification", content);

        // Assert
        var contentStr = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, contentStr);

        var lawyer = await DbContext.Users.OfType<Lawyer>().FirstOrDefaultAsync(u => u.Email == email);
        lawyer!.CurrentOnboardingStep.Should().Be(4);
    }

    [Fact]
    public async Task SubmitForReview_ShouldReturnOk_WhenAllStepsCompleted()
    {
        // Arrange
        var email = "onboarding.submit@test.com";
        var client = await AuthHelper.AuthenticateUserAsync(Factory, email, "Password123!", "Lawyer");

        // We can manually set the onboarding step to 4 to simulate completion of previous steps
        var user = await DbContext.Users.OfType<Lawyer>().FirstOrDefaultAsync(u => u.Email == email);
        user!.CurrentOnboardingStep = 4;
        user.CompletedOnboardingSteps = new List<int> { 1, 2, 3, 4 };
        await DbContext.SaveChangesAsync();

        // Act
        var response = await client.PostAsync("/api/lawyer/onboarding/submit-for-review", null);

        // Assert
        var contentStr = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, contentStr);

        var lawyer = await DbContext.Users.OfType<Lawyer>().FirstOrDefaultAsync(u => u.Email == email);
        await DbContext.Entry(lawyer!).ReloadAsync();
        lawyer!.CurrentOnboardingStep.Should().Be(-1); // Status indicating 'Submitted For Review'
        lawyer.VerificationStatus.Should().Be(VerificationStatus.UnderReview);
    }
}

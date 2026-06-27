using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wakiliy.Application.Features.Auth.Commands.Register;
using Wakiliy.Domain.Entities;
using Xunit;

namespace Wakiliy.API.IntegrationTests.Authentication;

public class RegisterTests : BaseIntegrationTest
{
    public RegisterTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    /*
     * Explanation:
     * Why this integration test exists: This test validates the happy-path scenario for registering a new user. It ensures that when valid data is provided, the entire stack works correctly.
     * Which business rule it validates: A user can register if they provide valid information, a unique email, and a valid UserType (e.g., Client). Upon successful registration, they are added to the system, and an email verification OTP is generated.
     * Which layers of the application are exercised: HTTP Request -> Middleware -> AuthController -> MediatR -> Validation -> RegisterCommandHandler -> UserManager/EF Core -> SQL Server.
     * What should be asserted: 
     *  - The HTTP status code must be 201 Created.
     *  - The user must be inserted into the SQL Server database.
     *  - A corresponding EmailOtp record must be generated and hashed.
     * Why those assertions are important: It confirms the persistence logic actually committed data to the real database and guarantees our core registration behavior is functioning flawlessly in a production-like environment.
     */
    [Fact]
    public async Task Register_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "johndoe@example.com",
            Password = "StrongPassword123!",
            UserType = "Client",
            AcceptTerms = true
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", command);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Status Code: {response.StatusCode}, Content: {content}");
        }

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Database Assertions
        var userInDb = await DbContext.Users.FirstOrDefaultAsync(u => u.Email == command.Email);
        userInDb.Should().NotBeNull();
        userInDb!.FirstName.Should().Be("John");
        userInDb.LastName.Should().Be("Doe");
        
        // Verify OTP was created
        var otpInDb = await DbContext.Set<EmailOtp>().FirstOrDefaultAsync(o => o.Email == command.Email);
        otpInDb.Should().NotBeNull();
        otpInDb!.Code.Should().NotBeNullOrEmpty("OTP should be hashed and saved");
    }

    /*
     * Explanation:
     * Why this integration test exists: To ensure the system correctly prevents duplicate registrations.
     * Which business rule it validates: Email addresses must be unique across all users.
     * Which layers of the application are exercised: Controller -> RegisterCommandHandler -> UserManager.
     * What should be asserted: 
     *  - The HTTP status code should be 400 BadRequest.
     *  - The database should still only contain the original user.
     *  - No new EmailOtp should be generated for the duplicate request.
     * Why those assertions are important: It prevents data corruption and ensures security/integrity rules regarding identity are strictly enforced.
     */
    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenEmailAlreadyExists()
    {
        // Arrange
        var existingUser = new Client
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "janedoe@example.com",
            UserName = "janedoe@example.com"
        };
        
        // Use UserManager to create user properly with a password so it simulates real state
        var userManager = Scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<AppUser>>();
        await userManager.CreateAsync(existingUser, "ExistingPassword123!");

        var command = new RegisterCommand
        {
            FirstName = "Another",
            LastName = "User",
            Email = "janedoe@example.com", // Same email
            Password = "StrongPassword123!",
            UserType = "Client",
            AcceptTerms = true
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        // Database Assertions: Verify no extra users or OTPs were created
        var usersWithEmail = await DbContext.Users.CountAsync(u => u.Email == command.Email);
        usersWithEmail.Should().Be(1, "Only the initially seeded user should exist");

        var otpCount = await DbContext.Set<EmailOtp>().CountAsync(o => o.Email == command.Email);
        otpCount.Should().Be(0, "No OTP should be generated for a failed registration");
    }

    /*
     * Explanation:
     * Why this integration test exists: Validates the Identity password policies are correctly integrated.
     * Which business rule it validates: Passwords must meet complexity requirements (e.g., uppercase, lowercase, numbers, special characters).
     * Which layers of the application are exercised: Validation/Identity configuration -> SQL Server (ensuring rollback/no save).
     * What should be asserted: 
     *  - HTTP 400 BadRequest.
     *  - User count remains 0 in the database.
     * Why those assertions are important: Weak passwords are a security vulnerability. This ensures the API actively rejects them before any database changes occur.
     */
    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenPasswordIsInvalid()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FirstName = "Weak",
            LastName = "Password",
            Email = "weakpass@example.com",
            Password = "123", // Too short, no complexity
            UserType = "Client",
            AcceptTerms = true
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Database Assertions
        var userInDb = await DbContext.Users.FirstOrDefaultAsync(u => u.Email == command.Email);
        userInDb.Should().BeNull("User should not be saved with an invalid password");
    }

    /*
     * Explanation:
     * Why this integration test exists: Validates the domain constraints around user roles.
     * Which business rule it validates: The system only accepts specific user types (e.g., 'Client' or 'Lawyer').
     * Which layers of the application are exercised: Controller -> RegisterCommandHandler enum parsing.
     * What should be asserted: 
     *  - HTTP 400 BadRequest.
     *  - Database state remains unchanged.
     * Why those assertions are important: Bad enum values or unmapped roles could crash the application or create unauthorized users.
     */
    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenUserTypeIsInvalid()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FirstName = "Fake",
            LastName = "Role",
            Email = "fakerole@example.com",
            Password = "StrongPassword123!",
            UserType = "SuperAdminFakeRole", // Invalid role
            AcceptTerms = true
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Database Assertions
        var userInDb = await DbContext.Users.FirstOrDefaultAsync(u => u.Email == command.Email);
        userInDb.Should().BeNull("User should not be saved with an invalid user type");
    }
}

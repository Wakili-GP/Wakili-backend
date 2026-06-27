using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Wakiliy.Application.Features.Auth.Commands.AdminLogin;
using Wakiliy.Application.Features.Auth.Commands.Login;
using Wakiliy.Application.Features.Auth.Commands.Register;
using Wakiliy.Domain.Entities;
using Wakiliy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Wakiliy.API.Common;
using Wakiliy.Application.Features.Auth.DTOs;
using Wakiliy.Domain.Responses;
using Microsoft.AspNetCore.Identity;
using Wakiliy.Domain.Constants;

namespace Wakiliy.API.IntegrationTests.Authentication.Helpers;

public static class AuthHelper
{
    public static async Task<HttpResponseMessage> RegisterUserAsync(HttpClient client, string email, string password, string userType)
    {
        var command = new RegisterCommand
        {
            FirstName = "Test",
            LastName = "User",
            Email = email,
            Password = password,
            UserType = userType,
            AcceptTerms = true
        };

        return await client.PostAsJsonAsync("/api/auth/register", command);
    }

    public static async Task<HttpResponseMessage> LoginAsync(HttpClient client, string email, string password)
    {
        var command = new LoginCommand
        {
            Email = email,
            Password = password
        };

        return await client.PostAsJsonAsync("/api/auth/login", command);
    }

    public static async Task<HttpResponseMessage> AdminLoginAsync(HttpClient client, string email, string password)
    {
        var command = new AdminLoginCommand
        {
            Email = email,
            Password = password
        };

        return await client.PostAsJsonAsync("/api/auth/admin-login", command);
    }

    public static async Task VerifyEmailAsync(ApplicationDbContext dbContext, string email)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user != null)
        {
            user.EmailConfirmed = true;
            await dbContext.SaveChangesAsync();
        }
    }

    public static async Task<HttpClient> AuthenticateUserAsync(CustomWebApplicationFactory factory, string email, string password, string userType)
    {
        var client = factory.CreateClient();
        
        // 1. Register
        await RegisterUserAsync(client, email, password, userType);

        // 2. Verify Email via DB to bypass OTP
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await VerifyEmailAsync(dbContext, email);

        // 3. Login
        var loginResponse = await LoginAsync(client, email, password);
        loginResponse.EnsureSuccessStatusCode();

        var content = await loginResponse.Content.ReadFromJsonAsync<SuccessResponse<LoginResponse>>();
        var token = content!.Data!.AccessToken;

        // 4. Create new client with token
        var authenticatedClient = factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return authenticatedClient;
    }

    public static async Task<HttpClient> AuthenticateAdminAsync(CustomWebApplicationFactory factory, string email, string password)
    {
        var client = factory.CreateClient();
        var loginResponse = await AdminLoginAsync(client, email, password);
        loginResponse.EnsureSuccessStatusCode();

        var content = await loginResponse.Content.ReadFromJsonAsync<SuccessResponse<LoginResponse>>();
        var token = content!.Data!.AccessToken;

        var authenticatedClient = factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return authenticatedClient;
    }
}

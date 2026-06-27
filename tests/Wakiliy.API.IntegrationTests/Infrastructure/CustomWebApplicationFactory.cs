using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Wakiliy.Infrastructure.Data;
using Xunit;

namespace Wakiliy.API.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer;

    public CustomWebApplicationFactory()
    {
        _dbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Strong_password_123!")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var testConfig = new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", _dbContainer.GetConnectionString() },
                { "Jwt:Key", "SuperSecretKeyForIntegrationTesting1234567890!" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "MailSettings:Host", "smtp.test.com" },
                { "MailSettings:Port", "587" },
                { "MailSettings:Email", "test@test.com" },
                { "MailSettings:Password", "password" },
                { "MailSettings:DisplayName", "Test Display" },
                { "SuperAdminUser:Email", "superadmin@test.com" },
                { "SuperAdminUser:Password", "StrongPassword123!" },
                { "SuperAdminUser:UserName", "superadmin@test.com" },
                { "AdminUser:Email", "admin@test.com" },
                { "AdminUser:Password", "StrongPassword123!" },
                { "AdminUser:UserName", "admin@test.com" },
                { "LawyerUser:Email", "lawyer@test.com" },
                { "LawyerUser:Password", "StrongPassword123!" },
                { "LawyerUser:UserName", "lawyer@test.com" },
                { "ClientUser:Email", "client@test.com" },
                { "ClientUser:Password", "StrongPassword123!" },
                { "ClientUser:UserName", "client@test.com" }
            };

            configBuilder.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureServices(services =>
        {
            GlobalConfiguration.Configuration.UseMemoryStorage();
            services.AddHangfire(config => config.UseMemoryStorage());

            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKeyForIntegrationTesting1234567890!"));
                options.TokenValidationParameters.ValidIssuer = "TestIssuer";
                options.TokenValidationParameters.ValidAudience = "TestAudience";
            });

            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(_dbContainer.GetConnectionString()));
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}

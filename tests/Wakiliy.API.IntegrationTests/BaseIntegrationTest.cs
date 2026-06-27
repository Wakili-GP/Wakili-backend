using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Wakiliy.API.IntegrationTests.Infrastructure;
using Wakiliy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Wakiliy.Infrastructure.Data.Seed;

namespace Wakiliy.API.IntegrationTests;

[Collection("IntegrationTests")]
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly IServiceScope Scope;
    protected readonly ApplicationDbContext DbContext;
    private readonly string _connectionString;

    protected BaseIntegrationTest(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
        Scope = factory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _connectionString = DbContext.Database.GetConnectionString()!;
    }

    public async Task InitializeAsync()
    {
        await DatabaseResetter.InitializeAsync(_connectionString);
        await DatabaseResetter.ResetAsync(_connectionString);
        await DbSeeder.MigrateAndSeedAsync(Scope.ServiceProvider);
    }

    public Task DisposeAsync()
    {
        Scope.Dispose();
        return Task.CompletedTask;
    }
}

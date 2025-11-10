using Wakiliy.API.Extensions;
using Wakiliy.Application.Extensions;
using Wakiliy.Infrastructure.Data.Seed;
using Wakiliy.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Cleanly register infrastructure , application , and presentation layers 
builder.Services
    .AddPresentaion(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

var app = builder.Build();

#region Migrate And Seed Users
// Seeding: Apply pending EF Core migrations and seed required roles/admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbSeeder.MigrateAndSeedAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during DB setup: {ex.Message}");
    }
} 
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

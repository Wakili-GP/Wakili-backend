using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

        services.AddDbContext<ApplicationDbContext>(options =>
           options.UseSqlServer(connectionString));

        // register repositories and seeders

        //services.AddScoped<IRestaurantSeeder, RestaurantSeeder>();
        //services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        //services.AddScoped<IDishRepository, DishRepository>();


        return services;
    }
}
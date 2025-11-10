using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wakiliy.Application.Common.Settings;
using Wakiliy.Application.Interfaces.Services;
using Wakiliy.Infrastructure.Authentication;
using Wakiliy.Infrastructure.Data;
using Wakiliy.Infrastructure.Services;

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

        // Register Services
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<IJwtProvider, JwtProvider>();


        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
        services.AddHttpContextAccessor();

        return services;
    }
}
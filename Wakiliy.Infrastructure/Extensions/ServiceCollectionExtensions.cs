using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wakiliy.Application.Common.Interfaces;
using Wakiliy.Application.Common.Settings;
using Wakiliy.Application.Interfaces.Services;
using Wakiliy.Application.Repositories;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Authentication;
using Wakiliy.Infrastructure.Data;
using Wakiliy.Infrastructure.Repositories;
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

        services.AddScoped<ILawyerRepository, LawyerRepository>();
        services.AddScoped<ISpecializationRepository, SpecializationRepository>();
        services.AddScoped<IEmailOtpRepository, EmailOtpRepository>();
        services.AddScoped<IUploadedFileRepository, UploadedFileRepository>();
        

        // Register Services
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<IJwtProvider, JwtProvider>();


        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
        services.AddHttpContextAccessor();


        // configure cloudinary settings and register the Cloudinary client
        services.Configure<CloudinarySettings>(
            configuration.GetSection("Cloudinary"));

        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;

            return new Cloudinary(new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret));
        });

        services.AddScoped<IFileUploadService, CloudinaryFileUploadService>();

        return services;
    }
}
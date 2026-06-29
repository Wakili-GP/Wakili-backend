using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Hangfire;
using Polly;
using Polly.Extensions.Http;
using Wakiliy.Application.Common.Interfaces;
using Wakiliy.Application.Common.Settings;
using Wakiliy.Application.Interfaces;
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
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<ISpecializationRepository, SpecializationRepository>();
        services.AddScoped<IEmailOtpRepository, EmailOtpRepository>();
        services.AddScoped<IUploadedFileRepository, UploadedFileRepository>();
        services.AddScoped<IAcademicQualificationRepository, AcademicQualificationRepository>();
        services.AddScoped<IProfessionalCertificationRepository, ProfessionalCertificationRepository>();
        services.AddScoped<IVerificationDocumentRepository, VerificationDocumentRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IFavoriteLawyerRepository, FavoriteLawyerRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAppointmentSlotRepository, AppointmentSlotRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IBookingIntentRepository, BookingIntentRepository>();
        services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
        services.AddScoped<ILawyerEarningRepository, LawyerEarningRepository>();
        services.AddScoped<IPayrollRepository, PayrollRepository>();
        services.AddScoped<IForumRepository, ForumRepository>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();



        // Register Services
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddHttpClient<IAiReviewAnalysisService, AiReviewAnalysisService>(client =>
        {
            client.BaseAddress = new Uri("https://nouraelkashif83--wakili-classifier-fastapi-app.modal.run/");
            client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddPolicyHandler((sp, request) =>
        {
            var logger = sp.GetRequiredService<ILogger<AiReviewAnalysisService>>();
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        var msg = outcome.Exception?.Message ?? $"Status Code: {outcome.Result?.StatusCode}";
                        logger.LogWarning("Delaying for {Delay}ms, then making retry {Retry}. Reason: {Reason}", timespan.TotalMilliseconds, retryAttempt, msg);
                    });
        })
        .AddPolicyHandler((sp, request) =>
        {
            var logger = sp.GetRequiredService<ILogger<AiReviewAnalysisService>>();
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TaskCanceledException>()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromMinutes(1),
                    onBreak: (outcome, state, timespan, context) =>
                    {
                        var msg = outcome.Exception?.Message ?? $"Status Code: {outcome.Result?.StatusCode}";
                        logger.LogWarning("Circuit Opened for {Duration} minutes. Reason: {Reason}", timespan.TotalMinutes, msg);
                    },
                    onReset: (context) =>
                    {
                        logger.LogInformation("Circuit Closed - Requests are flowing normally.");
                    },
                    onHalfOpen: () =>
                    {
                        logger.LogInformation("Circuit Half-Open - Testing next request.");
                    });
        });

        services.AddHttpClient<IAiLawyerVerificationService, AiLawyerVerificationService>(client =>
        {
            client.BaseAddress = new Uri("https://nouraelkashif83--wakili-lawyer-verification-fastapi-app.modal.run/");
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddPolicyHandler((sp, request) =>
        {
            var logger = sp.GetRequiredService<ILogger<AiLawyerVerificationService>>();
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        var msg = outcome.Exception?.Message ?? $"Status Code: {outcome.Result?.StatusCode}";
                        logger.LogWarning("Delaying for {Delay}ms, then making retry {Retry}. Reason: {Reason}", timespan.TotalMilliseconds, retryAttempt, msg);
                    });
        })
        .AddPolicyHandler((sp, request) =>
        {
            var logger = sp.GetRequiredService<ILogger<AiLawyerVerificationService>>();
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TaskCanceledException>()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromMinutes(1),
                    onBreak: (outcome, state, timespan, context) =>
                    {
                        var msg = outcome.Exception?.Message ?? $"Status Code: {outcome.Result?.StatusCode}";
                        logger.LogWarning("Circuit Opened for {Duration} minutes. Reason: {Reason}", timespan.TotalMinutes, msg);
                    },
                    onReset: (context) =>
                    {
                        logger.LogInformation("Circuit Closed - Requests are flowing normally.");
                    },
                    onHalfOpen: () =>
                    {
                        logger.LogInformation("Circuit Half-Open - Testing next request.");
                    });
        });
        
        // Paymob setup
        services.Configure<PaymobSettings>(configuration.GetSection(nameof(PaymobSettings)));
        services.AddHttpClient<IPaymobService, PaymobService>(client =>
        {
            client.BaseAddress = new Uri("https://accept.paymob.com/api/");
        });


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

        // Hangfire setup
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString));

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = Environment.ProcessorCount * 2;
        });

        return services;
    }
}
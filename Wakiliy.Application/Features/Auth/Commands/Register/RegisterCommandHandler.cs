using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using Wakiliy.Application.Features.Auth.DTOs;
using Wakiliy.Application.Helpers;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;
using static System.Net.WebRequestMethods;

namespace Wakiliy.Application.Features.Auth.Commands.Register;
public class RegisterCommandHandler(UserManager<AppUser> userManager,
    IEmailOtpRepository emailOtpRepository,
    ILogger<RegisterCommandHandler> logger,
    IHttpContextAccessor httpContextAccessor,
    IEmailSender emailSender) : IRequestHandler<RegisterCommand,Result>
{
    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<UserType>(request.UserType, true, out var userTypeEnum))
        {
            return Result.Failure(AuthErrors.InvalidUserType);
        }

        var emailExists =await userManager.Users.AnyAsync(u=>u.Email==request.Email);

        if (emailExists)
            return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);

        AppUser user;
        if (userTypeEnum == UserType.Lawyer)
            user = request.Adapt<Lawyer>();
        else
            user = request.Adapt<Client>();



        user.UserName = request.Email;
        user.NormalizedUserName = request.Email.ToUpper();

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var error = string.Join(',', result.Errors.Select(e => e.Description));
            return Result.Failure<AuthResponse>(new Error("User.InvalidPassword", error, StatusCodes.Status400BadRequest));
        }

        // Assign role to user
        var role = userTypeEnum == UserType.Lawyer ? DefaultRoles.Lawyer : DefaultRoles.Client;

        await userManager.AddToRoleAsync(user, role);


        // Send confirmation email to user
        var code = GenerateRandomNumber();

        logger.LogInformation("ConfirmationCode: {Code}", code);

        var emailOtp = new EmailOtp
        {
            Email = user.Email!,
            Code = HashOtp(code),
            ExpireAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            Purpose = OtpPurpose.EmailVerification
        };

        await emailOtpRepository.AddAsync(emailOtp);
        await emailOtpRepository.SaveChangesAsync();


        await SendConfirmationEmail(user, code);

        return Result.Success();

    }
    private string GenerateRandomNumber()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    private async Task SendConfirmationEmail(AppUser user, string otp)
    {
        var origin = httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var tokens = new Dictionary<string, string>
        {
            { "{{name}}", $"{user.FirstName} {user.LastName}" },
            { "{{otp}}", otp },
        };

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",tokens);

        await emailSender.SendEmailAsync(user.Email!, "Your verification code", emailBody);
        //BackgroundJob.Enqueue(() => emailSender.SendEmailAsync(user.Email!, "Confirm your email", emailBody));

        await Task.CompletedTask;
    }

    private string HashOtp(string otp)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(otp));
        return Convert.ToBase64String(bytes);
    }
}

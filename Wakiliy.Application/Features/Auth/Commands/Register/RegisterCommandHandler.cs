using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;
using Wakiliy.Application.Features.Auth.DTOs;
using Wakiliy.Application.Helpers;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Auth.Commands.Register;
public class RegisterCommandHandler(UserManager<AppUser> userManager,
    ILogger<RegisterCommandHandler> logger,
    IHttpContextAccessor httpContextAccessor,
    IEmailSender emailSender) : IRequestHandler<RegisterCommand,Result>
{
    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailExists =await userManager.Users.AnyAsync(u=>u.Email==request.Email);

        if (emailExists)
            return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);

        var user = request.Adapt<AppUser>();

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var error = string.Join(',', result.Errors.Select(e => e.Description));
            return Result.Failure<AuthResponse>(new Error("User.InvalidPassword", error, StatusCodes.Status400BadRequest));
        }

        // Assign role to user
        var role = request.UserType switch
        {
            UserType.Lawyer => DefaultRoles.Lawyer,
            UserType.Client => DefaultRoles.Client,
            _ => throw new Exception("Invalid user type")
        };

        await userManager.AddToRoleAsync(user, role);


        // Send confirmation email to user
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        logger.LogInformation("ConfirmationCode: {Code}", code);

        await SendConfirmationEmail(user, code);

        return Result.Success();

    }

    private async Task SendConfirmationEmail(AppUser user, string code)
    {
        var origin = httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            new Dictionary<string, string>
            {
                { "{{name}}", user.FullName },
                { "{{action_url}}", $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
            });

        await emailSender.SendEmailAsync(user.Email!, "Confirm your email", emailBody);
        //BackgroundJob.Enqueue(() => emailSender.SendEmailAsync(user.Email!, "Confirm your email", emailBody));

        await Task.CompletedTask;
    }
}

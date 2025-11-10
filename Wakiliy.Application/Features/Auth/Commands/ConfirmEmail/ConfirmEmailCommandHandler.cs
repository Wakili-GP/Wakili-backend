using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Auth.Commands.ConfirmEmail;
public class ConfirmEmailCommandHandler(UserManager<AppUser> userManager,ILogger<ConfirmEmailCommandHandler> logger) : IRequestHandler<ConfirmEmailCommand, Result>
{
    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);

        if (user is null)
            return Result.Failure(UserErrors.InvalidCode);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailAlreadyConfirmed);

        string decodedCode;
        try
        {
            decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await userManager.ConfirmEmailAsync(user, decodedCode);

        if (!result.Succeeded)
        {
            logger.LogInformation("Email confirmation failed for user {UserId}. Errors: {Errors}", request.UserId, string.Join(',', result.Errors.Select(e => e.Description)));
            var error = string.Join(',', result.Errors.Select(e => e.Description));
            return Result.Failure(new Error("User.InvalidCode", error, StatusCodes.Status400BadRequest));
        }

        return Result.Success();
    }
}

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Auth.Commands.ConfirmEmail;
public class ConfirmEmailCommandHandler(UserManager<AppUser> userManager,ILogger<ConfirmEmailCommandHandler> logger,IEmailOtpRepository emailOtpRepository) : IRequestHandler<ConfirmEmailCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result.Failure<string>(UserErrors.InvalidCode);

        if (user.EmailConfirmed)
            return Result.Failure<string>(AuthErrors.EmailAlreadyVerified);

        var hashedOtp = HashOtp(request.Code);

        var otpEntity = await emailOtpRepository
            .GetValidOtpAsync(request.Email, hashedOtp,OtpPurpose.EmailVerification);

        if (otpEntity is null)
            return Result.Failure<string>(AuthErrors.InvalidOtp);

        if (otpEntity.ExpireAt < DateTime.UtcNow)
            return Result.Failure<string>(AuthErrors.ExpiredOtp);

        otpEntity.IsUsed = true;
        user.EmailConfirmed = true;

        await emailOtpRepository.SaveChangesAsync();
        await userManager.UpdateAsync(user);

        return Result.Success("Email verified successfully");
    }

    private static string HashOtp(string otp)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(otp));
        return Convert.ToBase64String(bytes);
    }
}

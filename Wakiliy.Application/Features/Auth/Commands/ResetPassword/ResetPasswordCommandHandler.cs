using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordHandler(
        UserManager<AppUser> userManager,
        IEmailOtpRepository emailOtpRepository
    ) : IRequestHandler<ResetPasswordCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken ct)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return Result.Failure<string>(UserErrors.UserNotFound);

            var hashedOtp = HashOtp(request.Code);

            var otpEntity = await emailOtpRepository.GetValidOtpAsync(request.Email,hashedOtp,OtpPurpose.PasswordReset);

            if (otpEntity is null)
                return Result.Failure<string>(AuthErrors.InvalidOtp);

            if (otpEntity.ExpireAt < DateTime.UtcNow)
                return Result.Failure<string>(AuthErrors.ExpiredOtp);

            // reset password using Identity
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user,resetToken,request.NewPassword);

            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure<string>(new Error(error.Code,error.Description,StatusCodes.Status400BadRequest));
            }

            otpEntity.IsUsed = true;
            await emailOtpRepository.SaveChangesAsync();

            return Result.Success("Password reset successfully");
        }

        private static string HashOtp(string otp)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(
                sha.ComputeHash(Encoding.UTF8.GetBytes(otp)));
        }
    }

}

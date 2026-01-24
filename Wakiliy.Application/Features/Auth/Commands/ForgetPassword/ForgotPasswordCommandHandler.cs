using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Security.Cryptography;
using System.Text;
using Wakiliy.Application.Helpers;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler(
        UserManager<AppUser> userManager,
        IEmailSender emailSender,
        IEmailOtpRepository emailOtpRepository) : IRequestHandler<ForgetPasswordCommand, Result>
    {
        public async Task<Result> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Result.Success();

            await emailOtpRepository.InvalidatePreviousAsync(user.Email!);

            var otp = Random.Shared.Next(100000, 999999).ToString();
            var hashedOtp = HashOtp(otp);

            await emailOtpRepository.AddAsync(new EmailOtp
            {
                Email = request.Email,
                Code = hashedOtp,
                ExpireAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                Purpose = OtpPurpose.PasswordReset
            });

            await emailOtpRepository.SaveChangesAsync();

            await SendResetPasswordOtpEmail(user, otp,emailSender);

            return Result.Success();
        }

        private static string HashOtp(string otp)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(
                sha.ComputeHash(Encoding.UTF8.GetBytes(otp)));
        }

        private static async Task SendResetPasswordOtpEmail(AppUser user,string otp,IEmailSender emailSender)
        {
            var tokens = new Dictionary<string, string>
            {
                { "{{name}}", user.FullName },
                { "{{otp}}", otp },
            };

            var emailBody = EmailBodyBuilder.GenerateEmailBody("ResetPasswordOtp",tokens);

            await emailSender.SendEmailAsync(user.Email!,"Reset your password",emailBody);
        }

    }
}
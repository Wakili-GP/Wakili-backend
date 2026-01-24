using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Application.Helpers;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Wakiliy.Application.Features.Auth.Commands.ResendConfirmEmail
{
    public class ResendConfirmEmailCommandHandler(
        UserManager<AppUser> userManager,
        IEmailOtpRepository emailOtpRepository,
        IEmailSender emailSender,
        IHttpContextAccessor httpContextAccessor
        ) : IRequestHandler<ResendConfirmEmailCommand, Result>
    {
        public async Task<Result> Handle(ResendConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return Result.Failure(UserErrors.UserNotFound);

            if (user.EmailConfirmed)
                return Result.Failure(AuthErrors.EmailAlreadyVerified);

            var canResend = await emailOtpRepository.CanResendAsync(request.Email);
            if (!canResend)
                return Result.Failure(AuthErrors.ResendTooSoon);

            await emailOtpRepository.InvalidatePreviousAsync(request.Email);

            // generate code then save it into db and send confirmation email
            var code = GenerateRandomNumber();

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
            { "{{name}}", user.FullName },
            { "{{otp_1}}", otp[0].ToString() },
            { "{{otp_2}}", otp[1].ToString() },
            { "{{otp_3}}", otp[2].ToString() },
            { "{{otp_4}}", otp[3].ToString() },
            { "{{otp_5}}", otp[4].ToString() },
            { "{{otp_6}}", otp[5].ToString() }
        };

            var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation", tokens);

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
}

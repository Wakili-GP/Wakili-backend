using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Repositories
{
    public interface IEmailOtpRepository
    {
        Task AddAsync(EmailOtp otp);
        Task<EmailOtp?> GetValidOtpAsync(string email, string hashedOtp,OtpPurpose purpose);
        Task<bool> CanResendAsync(string email);
        Task InvalidatePreviousAsync(string email);
        Task SaveChangesAsync();
    }
}

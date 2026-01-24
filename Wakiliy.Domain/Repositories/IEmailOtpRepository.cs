using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Domain.Repositories
{
    public interface IEmailOtpRepository
    {
        Task AddAsync(EmailOtp otp);
        Task<EmailOtp?> GetValidOtpAsync(string email, string hashedOtp);
        Task SaveChangesAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories
{
    internal class EmailOtpRepository(ApplicationDbContext dbContext) : IEmailOtpRepository
    {
        public async Task AddAsync(EmailOtp otp)
        {
            await dbContext.AddAsync(otp);
        }

        public Task<EmailOtp?> GetValidOtpAsync(string email, string hashedOtp,OtpPurpose purpose)
        {
            return dbContext.EmailOtps.Where(x => x.Email == email && x.Code == hashedOtp && !x.IsUsed && x.Purpose==purpose)
                .OrderByDescending(x=>x.ExpireAt)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CanResendAsync(string email)
        {
            var lastOtp = await dbContext.EmailOtps
                .Where(x => x.Email == email)
                .OrderByDescending(x => x.ExpireAt)
                .FirstOrDefaultAsync();

            if (lastOtp is null)
                return true;

            return (DateTime.UtcNow - lastOtp.ExpireAt.AddMinutes(-5)).TotalSeconds > 60;
        }

        public async Task InvalidatePreviousAsync(string email)
        {
            var now = DateTime.UtcNow;

            var activeOtps = await dbContext.EmailOtps
                .Where(x => x.Email == email && !x.IsUsed && x.ExpireAt > now)
                .ToListAsync();

            if (!activeOtps.Any())
                return;

            foreach (var otp in activeOtps)
            {
                otp.IsUsed = true;
            }
        }


        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}

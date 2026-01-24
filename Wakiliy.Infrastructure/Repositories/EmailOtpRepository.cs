using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Domain.Entities;
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

        public Task<EmailOtp?> GetValidOtpAsync(string email, string hashedOtp)
        {
            return dbContext.EmailOtps.Where(x => x.Email == email && x.Code == hashedOtp && !x.IsUsed)
                .OrderByDescending(x=>x.ExpireAt)
                .FirstOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}

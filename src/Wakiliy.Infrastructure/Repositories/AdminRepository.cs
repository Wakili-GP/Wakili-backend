using Microsoft.EntityFrameworkCore;
using Wakiliy.Domain.Common.Models;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories
{
    public class AdminRepository(ApplicationDbContext dbContext) : IAdminRepository
    {
        public async Task<IEnumerable<AdminReadModel>> GetAdminsAsync()
        {
            var roles = new string[] { DefaultRoles.Admin, DefaultRoles.SuperAdmin };
            var query =
                from user in dbContext.Users
                join userRole in dbContext.UserRoles
                    on user.Id equals userRole.UserId
                join role in dbContext.Roles
                    on userRole.RoleId equals role.Id
                where roles.Contains(role.Name)
                select new AdminReadModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email!,
                    Role = role.Name!,
                    Status = user.Status.ToString(),
                    CreatedAt = user.CreatedAt
                };

            return await query.AsNoTracking().ToListAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Wakiliy.Domain.Common.Models;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories
{
    public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
    {
        public async Task<IEnumerable<UserReadModel>> GetUsersAsync()
        {
            var query =
                from user in dbContext.Users
                join userRole in dbContext.UserRoles
                    on user.Id equals userRole.UserId
                join role in dbContext.Roles
                    on userRole.RoleId equals role.Id
                where role.Name == DefaultRoles.Lawyer || role.Name == DefaultRoles.Client
                select new UserReadModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserType = role.Name!,
                    CreatedAt = user.CreatedAt,
                    LastActionDate = user.UpdatedAt,
                    Status = user.Status,
                    PhoneNumber = user.PhoneNumber
                };

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<UserReadModel?> GetUserByIdAsync(string id)
        {
            var query =
                from user in dbContext.Users
                join userRole in dbContext.UserRoles
                    on user.Id equals userRole.UserId
                join role in dbContext.Roles
                    on userRole.RoleId equals role.Id
                where (role.Name == DefaultRoles.Lawyer || role.Name == DefaultRoles.Client)
                      && user.Id == id
                select new UserReadModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserType = role.Name!,
                    CreatedAt = user.CreatedAt,
                    LastActionDate = user.UpdatedAt,
                    Status = user.Status,
                    PhoneNumber = user.PhoneNumber
                };

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }
    }
}

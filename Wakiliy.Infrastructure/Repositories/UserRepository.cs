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

        public async Task<(IEnumerable<UserReadModel> Users, int TotalCount)> GetUsersPagedAsync(int page, int pageSize, string? name, string? userType, UserStatus? status)
        {
            var query =
                from user in dbContext.Users
                join userRole in dbContext.UserRoles
                    on user.Id equals userRole.UserId
                join role in dbContext.Roles
                    on userRole.RoleId equals role.Id
                where role.Name == DefaultRoles.Lawyer || role.Name == DefaultRoles.Client
                select new { user, RoleName = role.Name };

            if (!string.IsNullOrWhiteSpace(name))
            {
                var searchName = name.ToLower();
                query = query.Where(x => 
                    x.user.FirstName.ToLower().Contains(searchName) || 
                    x.user.LastName.ToLower().Contains(searchName));
            }

            if (!string.IsNullOrWhiteSpace(userType))
            {
                var lowerUserType = userType.ToLower();
                query = query.Where(x => x.RoleName!.ToLower() == lowerUserType);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.user.Status == status.Value);
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(x => x.user.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new UserReadModel
                {
                    Id = x.user.Id,
                    FirstName = x.user.FirstName,
                    LastName = x.user.LastName,
                    Email = x.user.Email,
                    UserType = x.RoleName!,
                    CreatedAt = x.user.CreatedAt,
                    LastActionDate = x.user.UpdatedAt,
                    Status = x.user.Status,
                    PhoneNumber = x.user.PhoneNumber
                })
                .AsNoTracking()
                .ToListAsync();

            return (users, totalCount);
        }
    }
}

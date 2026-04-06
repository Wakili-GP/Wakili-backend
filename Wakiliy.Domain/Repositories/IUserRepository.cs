using Wakiliy.Domain.Common.Models;

using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserReadModel>> GetUsersAsync();
        Task<UserReadModel?> GetUserByIdAsync(string id);
        Task<(IEnumerable<UserReadModel> Users, int TotalCount)> GetUsersPagedAsync(int page, int pageSize, string? name, string? userType, UserStatus? status);
    }
}

using Wakiliy.Domain.Common.Models;

namespace Wakiliy.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserReadModel>> GetUsersAsync();
        Task<UserReadModel?> GetUserByIdAsync(string id);
    }
}

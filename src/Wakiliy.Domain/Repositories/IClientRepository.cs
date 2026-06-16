using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Domain.Repositories
{
    public interface IClientRepository
    {
        Task<Client?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<int> UpdateAsync(Client client, CancellationToken cancellationToken = default);
    }
}

using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories
{
    public class ClientRepository(ApplicationDbContext dbContext) : IClientRepository
    {
        public async Task<Client?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Users
                .OfType<Client>()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<int> UpdateAsync(Client client, CancellationToken cancellationToken = default)
        {
            dbContext.Users.Update(client);
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

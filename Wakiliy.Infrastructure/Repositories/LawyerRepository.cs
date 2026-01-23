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
    public class LawyerRepository(ApplicationDbContext dbContext) : ILawyerRepository
    {
        public async Task<int> CreateAsync(Lawyer lawyer, CancellationToken cancellationToken)
        {
            await dbContext.Lawyers.AddAsync(lawyer);
            return await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> UpdateAsync(Lawyer lawyer, CancellationToken cancellationToken)
        {
            dbContext.Lawyers.Update(lawyer);
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

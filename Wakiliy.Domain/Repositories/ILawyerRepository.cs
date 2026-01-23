using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Domain.Repositories
{
    public interface ILawyerRepository
    {
        Task<int> CreateAsync(Lawyer lawyer, CancellationToken cancellationToken = default);
    }
}

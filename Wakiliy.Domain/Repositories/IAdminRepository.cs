using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Domain.Common.Models;

namespace Wakiliy.Domain.Repositories
{
    public interface IAdminRepository
    {
        Task<IEnumerable<AdminReadModel>> GetAdminsAsync();
    }
}

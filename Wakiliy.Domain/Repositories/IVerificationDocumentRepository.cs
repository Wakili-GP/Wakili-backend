using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wakiliy.Domain.Repositories
{
    public interface IVerificationDocumentRepository
    {
        Task DeleteByLawyerIdAsync(string lawyerId, CancellationToken cancellationToken);
    }
}

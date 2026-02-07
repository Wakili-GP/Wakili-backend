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
        Task<int> UpdateAsync(Lawyer lawyer, CancellationToken cancellationToken = default);
        Task<Lawyer?> GetByIdAsync(string id, CancellationToken cancellationToken=default);
        Task<Lawyer?> GetByIdWithQualificationsAndCertificationsAsync(string id, CancellationToken cancellationToken=default);
        Task<Lawyer?> GetByIdWithExperiencesAsync(string id, CancellationToken cancellationToken=default);
        Task<Lawyer?> GetLawyerWithVerificationAsync(string id, CancellationToken cancellationToken=default);
        Task<Lawyer?> GetByIdWithAllOnboardingDataAsync(string id);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Domain.Common.Models;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Repositories
{
    public interface ILawyerRepository
    {
        Task CreateAsync(Lawyer lawyer, CancellationToken cancellationToken = default);
        Task UpdateAsync(Lawyer lawyer, CancellationToken cancellationToken = default);
        Task<Lawyer?> GetByIdAsync(string id, CancellationToken cancellationToken=default);
        Task<Lawyer?> GetByIdWithQualificationsAndCertificationsAsync(string id, CancellationToken cancellationToken=default);
        Task<Lawyer?> GetByIdWithExperiencesAsync(string id, CancellationToken cancellationToken=default);
        Task<Lawyer?> GetLawyerWithVerificationAsync(string id, CancellationToken cancellationToken=default);
        Task<Lawyer?> GetByIdWithAllOnboardingDataAsync(string id);
        IQueryable<Lawyer> GetVerificationRequestsQueryable();
        Task<List<LawyerVerificationModel>> GetVerificationRequestsAsync(VerificationStatus? status, CancellationToken cancellationToken = default);
        Task<(IEnumerable<LawyerVerificationModel> Requests, int TotalCount, LawyerVerificationStats Stats)> GetVerificationRequestsPagedAsync(int page, int pageSize, string? search, VerificationStatus? status, CancellationToken cancellationToken = default);
        Task DeleteExperiencesByLawyerIdAsync(string lawyerId, CancellationToken cancellationToken = default);
    }
}

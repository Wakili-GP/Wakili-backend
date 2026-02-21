using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Domain.Common.Models;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
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

        public async Task<Lawyer?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await dbContext.Lawyers
                .Include(l => l.Specializations)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<Lawyer?> GetByIdWithQualificationsAndCertificationsAsync(string id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Lawyers
                .Include(l => l.AcademicQualifications)
                .Include(l => l.ProfessionalCertifications)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<Lawyer?> GetByIdWithExperiencesAsync(string id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Lawyers
                .Include(l => l.WorkExperiences)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<Lawyer?> GetLawyerWithVerificationAsync(string id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Lawyers
                .Include(l => l.VerificationDocuments)
                    //.ThenInclude(v => v.EducationalCertificates)
                .Include(l => l.VerificationDocuments)
                    //.ThenInclude(v => v.ProfessionalCertificates)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<Lawyer?> GetByIdWithAllOnboardingDataAsync(string id)
        {
            return await dbContext.Lawyers
                .Include(l => l.Specializations)
                .Include(l => l.AcademicQualifications)
                .Include(l => l.ProfessionalCertifications).ThenInclude(pc => pc.Document)
                .Include(l => l.WorkExperiences)
                .Include(l => l.VerificationDocuments).ThenInclude(vd => vd.File)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public IQueryable<Lawyer> GetVerificationRequestsQueryable()
        {
            return dbContext.Lawyers.AsNoTracking();
        }

        public async Task<List<LawyerVerificationModel>> GetVerificationRequestsAsync(
            VerificationStatus? status, 
            CancellationToken cancellationToken = default)
        {
            var query = dbContext.Lawyers
                .Include(l => l.Specializations)
                .AsNoTracking();

            if (status.HasValue)
            {
                query = query.Where(l => l.VerificationStatus == status.Value);
            }

            var result = await query
                .Select(l => new LawyerVerificationModel
                {
                    Id = l.Id,
                    FirstName = l.FirstName,
                    LastName = l.LastName,
                    Email = l.Email,
                    Specializations = string.Join(", ", l.Specializations.Select(s => s.Name)),
                    SubmittedAt = l.LastOnboardingUpdate,
                    ProfileImageUrl = l.ProfileImage.SystemFileUrl,
                    Status = l.VerificationStatus.ToString()
                })
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}

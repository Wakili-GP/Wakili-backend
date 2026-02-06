using Microsoft.EntityFrameworkCore;
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
                    .ThenInclude(v => v.EducationalCertificates)
                .Include(l => l.VerificationDocuments)
                    .ThenInclude(v => v.ProfessionalCertificates)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }
    }
}

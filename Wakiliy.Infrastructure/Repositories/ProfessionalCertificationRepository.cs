using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories
{
    internal class ProfessionalCertificationRepository : IProfessionalCertificationRepository
    {
        private readonly ApplicationDbContext _context;

        public ProfessionalCertificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task DeleteByLawyerIdAsync(
            string lawyerId,
            CancellationToken cancellationToken = default)
        {
            var certifications = await _context.ProfessionalCertifications
                .Include(c => c.Document)
                .Where(c => c.LawyerId == lawyerId)
                .ToListAsync(cancellationToken);

            if (certifications.Count == 0)
                return;

            _context.ProfessionalCertifications.RemoveRange(certifications);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

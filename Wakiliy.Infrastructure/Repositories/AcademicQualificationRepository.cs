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
    internal class AcademicQualificationRepository : IAcademicQualificationRepository
    {
        private readonly ApplicationDbContext _context;

        public AcademicQualificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
            
        public async Task DeleteByLawyerIdAsync(
            string lawyerId,
            CancellationToken cancellationToken = default)
        {
            var qualifications = await _context.AcademicQualifications
                .Where(q => q.LawyerId == lawyerId)
                .ToListAsync(cancellationToken);

            if (qualifications.Count == 0)
                return;

            _context.AcademicQualifications.RemoveRange(qualifications);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

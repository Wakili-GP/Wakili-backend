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
    internal class VerificationDocumentRepository : IVerificationDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public VerificationDocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task DeleteByLawyerIdAsync(string lawyerId, CancellationToken cancellationToken)
        {
            var docs = await _context.VerificationDocuments
                .Where(x => x.LawyerId == lawyerId)
                .ToListAsync(cancellationToken);

            if (docs.Count == 0)
                return;

            _context.VerificationDocuments.RemoveRange(docs);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

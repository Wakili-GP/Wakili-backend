using Microsoft.EntityFrameworkCore;
using Wakiliy.Application.Repositories;
using Wakiliy.Domain.Entities;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories
{
    public class UploadedFileRepository : IUploadedFileRepository
    {
        private readonly ApplicationDbContext _context;

        public UploadedFileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UploadedFile file, CancellationToken ct)
        {
            await _context.UploadedFiles.AddAsync(file, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task AddRangeAsync(IEnumerable<UploadedFile> files, CancellationToken ct)
        {
            await _context.UploadedFiles.AddRangeAsync(files, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<UploadedFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.UploadedFiles.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public async Task<List<UploadedFile>> GetByOwnerAsync(
            string ownerId,
            FilePurpose purpose,
            CancellationToken ct)
        {
            return await _context.UploadedFiles
                .Where(f => f.OwnerId == ownerId && f.Purpose == purpose)
                .ToListAsync(ct);
        }
    }

}

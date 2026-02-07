using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Repositories
{
    public interface IUploadedFileRepository
    {
        Task AddAsync(UploadedFile file, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<UploadedFile> files, CancellationToken cancellationToken = default);
        Task<List<UploadedFile>> GetByOwnerAsync(string ownerId,FilePurpose purpose,CancellationToken cancellationToken = default);

        Task<UploadedFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }

}

using Wakiliy.Domain.Entities;

namespace Wakiliy.Domain.Common;
public class AuditableEntity
{
    public string CreatedById { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public string? UpdatedById { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public AppUser CreatedBy { get; set; } = default!;
    public AppUser? UpdatedBy { get; set; }
}

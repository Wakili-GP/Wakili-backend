using System;

namespace Wakiliy.Domain.Entities;

public class WorkExperience
{
    public Guid Id { get; set; }
    public string LawyerId { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int? EndYear { get; set; }
    public bool IsCurrentJob { get; set; }
    public string? Description { get; set; }

    public Lawyer Lawyer { get; set; } = default!;
}

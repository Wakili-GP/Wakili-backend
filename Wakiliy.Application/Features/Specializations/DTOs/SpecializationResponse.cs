using System;

namespace Wakiliy.Application.Features.Specializations.DTOs;

public class SpecializationResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int NumOfLawyers { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedById { get; set; } = string.Empty;
    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedById { get; set; }
}

using System;
using System.Collections.Generic;
using Wakiliy.Domain.Common;

namespace Wakiliy.Domain.Entities;

public class Specialization : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<Lawyer> Lawyers { get; set; } = new List<Lawyer>();
}

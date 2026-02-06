using System;
using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Entities;

public class UploadedDocument
{
    public Guid Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; } = DocumentStatus.Uploaded;
}

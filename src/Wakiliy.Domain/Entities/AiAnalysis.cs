namespace Wakiliy.Domain.Entities;

public class AiAnalysis
{
    public bool IsFlagged { get; set; }
    public double Confidence { get; set; }
    public string Summary { get; set; } = string.Empty;
}

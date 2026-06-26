using System.Threading;
using System.Threading.Tasks;

namespace Wakiliy.Application.Common.Interfaces;

public class AiAnalysisResult
{
    public string Flag { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public double ConfidenceRate { get; set; }

    public bool IsFlagged => Flag != "Visible";
}

public interface IAiReviewAnalysisService
{
    Task<AiAnalysisResult> AnalyzeReviewAsync(string review, double rating, CancellationToken cancellationToken = default);
}

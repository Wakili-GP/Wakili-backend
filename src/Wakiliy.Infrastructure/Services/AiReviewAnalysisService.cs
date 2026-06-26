using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wakiliy.Application.Common.Interfaces;

namespace Wakiliy.Infrastructure.Services;

public class AiReviewAnalysisService : IAiReviewAnalysisService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AiReviewAnalysisService> _logger;

    public AiReviewAnalysisService(HttpClient httpClient, ILogger<AiReviewAnalysisService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<AiAnalysisResult> AnalyzeReviewAsync(string review, double rating, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("AI Review Analysis request started.");
        try
        {
            var requestBody = new
            {
                review = string.IsNullOrWhiteSpace(review) ? "بدون تعليق" : review,
                rating = rating
            };

            var response = await _httpClient.PostAsJsonAsync("classify", requestBody, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AiAnalysisResponse>(cancellationToken: cancellationToken);
                _logger.LogInformation("AI Review Analysis request completed successfully.");
                return new AiAnalysisResult
                {
                    Flag = result?.Flag ?? "Visible",
                    Comment = result?.Comment ?? string.Empty,
                    ConfidenceRate = result?.ConfidenceRate ?? 0
                };
            }
            else
            {
                _logger.LogWarning("AI Analysis service returned status code {StatusCode}", response.StatusCode);
            }
        }
        catch (Polly.CircuitBreaker.BrokenCircuitException)
        {
            _logger.LogWarning("AI Analysis service circuit is open. Falling back to default.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze review using AI service");
        }
        
        _logger.LogInformation("Returning final fallback for AI Review Analysis.");
        return new AiAnalysisResult
        {
            Flag = "Visible",
            Comment = "AI Analysis failed or is unavailable.",
            ConfidenceRate = 0
        };
    }
}

public class AiAnalysisResponse
{
    [JsonPropertyName("flag")]
    public string Flag { get; set; } = string.Empty;

    [JsonPropertyName("comment")]
    public string Comment { get; set; } = string.Empty;

    [JsonPropertyName("confidence_rate")]
    public double ConfidenceRate { get; set; }
}

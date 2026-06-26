using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wakiliy.Application.Common.Interfaces;

namespace Wakiliy.Infrastructure.Services;

public class AiLawyerVerificationService : IAiLawyerVerificationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AiLawyerVerificationService> _logger;

    public AiLawyerVerificationService(HttpClient httpClient, ILogger<AiLawyerVerificationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<LawyerVerificationResponse?> VerifyLawyerAsync(Guid lawyerId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("AI Lawyer Verification request started for lawyer {LawyerId}.", lawyerId);
        try
        {
            var response = await _httpClient.GetAsync($"verify-lawyer/{lawyerId}", cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LawyerVerificationResponse>(cancellationToken: cancellationToken);
                _logger.LogInformation("AI Lawyer Verification request completed successfully for lawyer {LawyerId}.", lawyerId);
                return result;
            }
            else
            {
                _logger.LogWarning("AI Lawyer Verification service returned status code {StatusCode} for lawyer {LawyerId}", response.StatusCode, lawyerId);
            }
        }
        catch (Polly.CircuitBreaker.BrokenCircuitException)
        {
            _logger.LogWarning("AI Lawyer Verification service circuit is open. Skipping verification.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify lawyer {LawyerId} using AI service", lawyerId);
        }
        
        return null;
    }
}

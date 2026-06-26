using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Wakiliy.Application.Common.Interfaces;

public class LawyerVerificationCheckDetails
{
    [JsonPropertyName("matched_parts")]
    public List<string> MatchedParts { get; set; } = new();

    [JsonPropertyName("missing_parts")]
    public List<string> MissingParts { get; set; } = new();

    [JsonPropertyName("match_ratio")]
    public double MatchRatio { get; set; }

    [JsonPropertyName("passed")]
    public bool Passed { get; set; }
}

public class LawyerVerificationChecks
{
    [JsonPropertyName("national_id")]
    public LawyerVerificationCheckDetails? NationalId { get; set; }

    [JsonPropertyName("lawyer_license")]
    public LawyerVerificationCheckDetails? LawyerLicense { get; set; }
}

public class LawyerVerificationResponse
{
    [JsonPropertyName("lawyer_id")]
    public string LawyerId { get; set; } = string.Empty;

    [JsonPropertyName("form_name")]
    public string FormName { get; set; } = string.Empty;

    [JsonPropertyName("checks")]
    public LawyerVerificationChecks? Checks { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    public bool IsValid => string.Equals(Status, "valid", StringComparison.OrdinalIgnoreCase);
}

public interface IAiLawyerVerificationService
{
    Task<LawyerVerificationResponse?> VerifyLawyerAsync(Guid lawyerId, CancellationToken cancellationToken = default);
}

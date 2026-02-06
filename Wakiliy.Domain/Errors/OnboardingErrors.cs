using Microsoft.AspNetCore.Http;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Domain.Errors;

public static class OnboardingErrors
{
    public static readonly Error LawyerNotFound = new("Onboarding.LawyerNotFound", "Unable to locate the current lawyer profile.", StatusCodes.Status404NotFound);

    public static Error StepPrerequisite(int requiredStep) => new(
        "Onboarding.StepPrerequisite",
        $"You must complete step {requiredStep} before continuing.",
        StatusCodes.Status400BadRequest);

    public static readonly Error InvalidDocumentStatus = new(
        "Onboarding.InvalidDocumentStatus",
        "One or more document statuses are invalid.",
        StatusCodes.Status400BadRequest);
}

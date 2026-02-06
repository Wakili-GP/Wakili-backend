using System;
using System.Collections.Generic;
using System.Linq;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Common;

public static class LawyerOnboardingHelper
{
    public static bool HasCompletedStep(this Lawyer lawyer, int step)
    {
        EnsureCollections(lawyer);
        return lawyer.CompletedOnboardingSteps.Contains(step);
    }

    public static bool CanAccessStep(this Lawyer lawyer, int step)
    {
        if (step <= 1)
        {
            return true;
        }

        EnsureCollections(lawyer);
        return lawyer.CompletedOnboardingSteps.Contains(step - 1);
    }

    public static void MarkStepCompleted(this Lawyer lawyer, int completedStep, int nextStep)
    {
        EnsureCollections(lawyer);

        if (!lawyer.CompletedOnboardingSteps.Contains(completedStep))
        {
            lawyer.CompletedOnboardingSteps.Add(completedStep);
        }

        lawyer.CompletedOnboardingSteps = lawyer.CompletedOnboardingSteps
            .Distinct()
            .Order()
            .ToList();

        lawyer.CurrentOnboardingStep = Math.Max(lawyer.CurrentOnboardingStep, nextStep);
        lawyer.LastOnboardingUpdate = DateTime.UtcNow;
    }

    public static OnboardingStepResponse<TData> BuildResponse<TData>(Lawyer lawyer, TData data, string message)
    {
        EnsureCollections(lawyer);

        return new OnboardingStepResponse<TData>
        {
            Message = message,
            Progress = new OnboardingProgressDto<TData>
            {
                CurrentStep = lawyer.CurrentOnboardingStep,
                CompletedSteps = lawyer.CompletedOnboardingSteps.ToArray(),
                Data = data,
                LastUpdated = lawyer.LastOnboardingUpdate
            }
        };
    }

    private static void EnsureCollections(Lawyer lawyer)
    {
        lawyer.CompletedOnboardingSteps ??= new List<int>();
        lawyer.SessionTypes ??= new List<string>();
        lawyer.Specializations ??= new List<Specialization>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.DTOs
{
    public class OnboardingProgressResponseDto
    {
        public int CurrentStep { get; set; }
        public IReadOnlyCollection<int> CompletedSteps { get; set; } = Array.Empty<int>();
        public OnboardingAllDataDto Data { get; set; } = new();
        public DateTime LastUpdated { get; set; }
    }

    public class OnboardingAllDataDto
    {
        public BasicInfoDataDto? BasicInfo { get; set; }
        public EducationDataDto? Education { get; set; }
        public ExperienceDataDto? Experience { get; set; }
        public VerificationDocumentsDto? Verification { get; set; }
    }
}

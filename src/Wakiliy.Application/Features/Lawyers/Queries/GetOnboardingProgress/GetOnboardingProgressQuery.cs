using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetOnboardingProgress
{
    public class GetOnboardingProgressQuery : IRequest<Result<OnboardingProgressResponseDto>>
    {
        public string UserId { get; }

        public GetOnboardingProgressQuery(string userId)
        {
            UserId = userId;
        }
    }
}

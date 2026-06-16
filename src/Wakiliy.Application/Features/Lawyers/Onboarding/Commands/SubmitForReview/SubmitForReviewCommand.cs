using MediatR;
using Wakiliy.Application.Features.Lawyers.Onboarding.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SubmitForReview;

public class SubmitForReviewCommand : IRequest<Result>
{
    public string UserId { get; set; } = string.Empty;
}

using Wakiliy.Domain.Responses;
using MediatR;

namespace Wakiliy.Application.Features.Reviews.Commands.Moderate;

public enum ReviewModerationAction
{
    RetryModeration,
    Approve,
    Hide
}

public class ModerateReviewCommand : IRequest<Result>
{
    public Guid ReviewId { get; set; }
    public ReviewModerationAction Action { get; set; }
}

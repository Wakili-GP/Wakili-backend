using MediatR;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Reviews.Commands.Create;

public class CreateReviewCommand : IRequest<Result<ReviewResponseDto>>
{
    public string UserId { get; set; } = string.Empty;
    public Guid AppointmentId { get; set; }
    public LawyerReviewDto LawyerReview { get; set; } = new();
    public SystemReviewDto? SystemReview { get; set; }
}

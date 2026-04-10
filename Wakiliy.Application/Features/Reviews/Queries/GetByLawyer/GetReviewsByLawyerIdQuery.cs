using MediatR;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Reviews.Queries.GetByLawyer;

public class GetReviewsByLawyerIdQuery : IRequest<Result<List<ReviewResponseDto>>>
{
    public string LawyerId { get; set; } = string.Empty;
}

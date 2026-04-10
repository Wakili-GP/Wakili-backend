using MediatR;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Reviews.Queries.GetAllSystemReviews;

public class GetAllSystemReviewsQuery : IRequest<Result<List<SystemReviewResponseDto>>>
{
}

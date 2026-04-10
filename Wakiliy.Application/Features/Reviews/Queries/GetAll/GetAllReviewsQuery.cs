using MediatR;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Reviews.Queries.GetAll;

public class GetAllReviewsQuery : IRequest<Result<List<ReviewResponseDto>>>
{
}

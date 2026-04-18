using MediatR;
using Wakiliy.Application.Common.Models;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Reviews.Queries.GetByLawyer;

public class GetReviewsByLawyerIdQuery : IRequest<Result<PaginatedResult<ReviewResponseDto>>>
{
    public string LawyerId { get; set; } = string.Empty;
    public double? Stars { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

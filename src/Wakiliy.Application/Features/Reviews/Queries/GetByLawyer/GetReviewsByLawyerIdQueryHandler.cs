using Mapster;
using MediatR;
using Wakiliy.Application.Common.Models;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Reviews.Queries.GetByLawyer;

public class GetReviewsByLawyerIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetReviewsByLawyerIdQuery, Result<PaginatedResult<ReviewResponseDto>>>
{
    public async Task<Result<PaginatedResult<ReviewResponseDto>>> Handle(GetReviewsByLawyerIdQuery request, CancellationToken cancellationToken)
    {
        var (reviews, totalCount) = await unitOfWork.Reviews.GetByLawyerIdPagedAsync(
            request.LawyerId,
            request.PageNumber,
            request.PageSize,
            request.Stars,
            request.SearchQuery,
            request.SortDescending,
            cancellationToken);

        var reviewDtos = reviews.Adapt<List<ReviewResponseDto>>();

        var paginatedResult = new PaginatedResult<ReviewResponseDto>
        {
            Items = reviewDtos,
            TotalCount = totalCount,
            Page = request.PageNumber,
            PageSize = request.PageSize
        };

        return Result.Success(paginatedResult);
    }
}

using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Application.Common.Models;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetApprovedLawyers;

public class GetApprovedLawyersQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetApprovedLawyersQuery, Result<PaginatedResult<LawyerResponse>>>
{
    public async Task<Result<PaginatedResult<LawyerResponse>>> Handle(
        GetApprovedLawyersQuery request, CancellationToken cancellationToken)
    {
        var sortByString = request.SortBy.HasValue ? request.SortBy.Value.ToString() : null;

        var query = unitOfWork.Lawyers.GetApprovedLawyersQuery(
            searchQuery: request.SearchQuery,
            specializationId: request.SpecializationId,
            city: request.City,
            minPrice: request.MinPrice,
            maxPrice: request.MaxPrice,
            minRating: request.MinRating,
            sessionTypes: request.SessionTypes,
            sortBy: sortByString,
            sortOrder: request.SortOrder
        );

        // Pagination 
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectToType<LawyerResponse>()
            .ToListAsync(cancellationToken);

        var result = new PaginatedResult<LawyerResponse>
        {
            Items      = items,
            Page       = request.Page,
            PageSize   = request.PageSize,
            TotalCount = totalCount
        };

        return Result.Success(result);
    }
}

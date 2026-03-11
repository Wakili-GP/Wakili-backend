using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Application.Common.Models;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetApprovedLawyers;

public class GetApprovedLawyersQueryHandler(UserManager<AppUser> userManager)
    : IRequestHandler<GetApprovedLawyersQuery, Result<PaginatedResult<LawyerResponse>>>
{
    public async Task<Result<PaginatedResult<LawyerResponse>>> Handle(
        GetApprovedLawyersQuery request, CancellationToken cancellationToken)
    {

        IQueryable<Lawyer> query = userManager.Users
            .OfType<Lawyer>()
            .Where(l => l.VerificationStatus == VerificationStatus.Approved && l.IsActive)
            .AsNoTracking();

        // Filters
        if (request.SpecializationId.HasValue)
            query = query.Where(l => l.Specializations.Any(s => s.Id == request.SpecializationId.Value));

        if (!string.IsNullOrWhiteSpace(request.City))
            query = query.Where(l => l.City.ToLower() == request.City.ToLower());

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(l => 
                (l.PhoneSessionPrice != null && l.PhoneSessionPrice <= request.MaxPrice.Value) ||
                (l.InOfficeSessionPrice != null && l.InOfficeSessionPrice <= request.MaxPrice.Value));
        }

        if (!string.IsNullOrWhiteSpace(request.SessionType))
            query = query.Where(l => l.SessionTypes.Contains(request.SessionType));

        // Sorting  
        query = request.SortByPrice?.ToLower() switch
        {
            "asc"  => query.OrderBy(l => l.PhoneSessionPrice).ThenBy(l => l.InOfficeSessionPrice),
            "desc" => query.OrderByDescending(l => l.PhoneSessionPrice).ThenByDescending(l => l.InOfficeSessionPrice),
            _      => query.OrderBy(l => l.JoinedDate)
        };

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

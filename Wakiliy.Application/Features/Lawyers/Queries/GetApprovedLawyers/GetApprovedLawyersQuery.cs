using MediatR;
using System.Collections.Generic;
using Wakiliy.Application.Common.Models;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetApprovedLawyers;

public enum LawyerSortBy
{
    Price,
    Rating
}

public class GetApprovedLawyersQuery
    : IRequest<Result<PaginatedResult<LawyerResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? SearchQuery { get; set; }

    public int? SpecializationId { get; set; }
    public string? City { get; set; }

    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    
    /// <summary>Minimum average rating filter (1-5)</summary>
    public double? MinRating { get; set; }

    /// <summary>Session type filter: 0 = InOffice, 1 = Phone</summary>
    public List<int>? SessionTypes { get; set; }

        /// <summary>Field to sort by: "Price" or "Rating". Null means no sorting.</summary>
    public LawyerSortBy? SortBy { get; set; }
     /// <summary>Sort order: "asc" or "desc". Null means no sorting.</summary>
    public string? SortOrder { get; set; } // "asc" or "desc"
}

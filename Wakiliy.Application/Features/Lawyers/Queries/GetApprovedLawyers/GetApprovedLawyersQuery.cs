using MediatR;
using Wakiliy.Application.Common.Models;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetApprovedLawyers;

public class GetApprovedLawyersQuery
    : IRequest<Result<PaginatedResult<LawyerResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    /// <summary>Sort by price: "asc" or "desc". Null means no sorting.</summary>
    public string? SortByPrice { get; set; }

    public int? SpecializationId { get; set; }
    public string? City { get; set; }

    /// <summary>Maximum price filter.</summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>Session type filter (e.g. "Online", "InPerson").</summary>
    public string? SessionType { get; set; }
}

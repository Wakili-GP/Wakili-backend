using MediatR;
using Wakiliy.Application.Common.Models;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetVerificationRequests
{
    public class GetLawyerVerificationRequestsQuery : IRequest<Result<PaginatedResult<LawyerVerificationRequestResponse>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public VerificationStatus? Status { get; set; }
    }
}

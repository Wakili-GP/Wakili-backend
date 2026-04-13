using Mapster;
using MediatR;
using Wakiliy.Application.Common.Models;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetVerificationRequests
{
    public class GetLawyerVerificationRequestsQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetLawyerVerificationRequestsQuery, Result<PaginatedResult<LawyerVerificationRequestResponse>>>
    {
        public async Task<Result<PaginatedResult<LawyerVerificationRequestResponse>>> Handle(
            GetLawyerVerificationRequestsQuery request,
            CancellationToken cancellationToken)
        {
            var (requests, totalCount,stats) = await unitOfWork.Lawyers.GetVerificationRequestsPagedAsync(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.Status,
                cancellationToken);

            var dtoItems = requests.Adapt<List<LawyerVerificationRequestResponse>>();

            var paginatedResult = new PaginatedResult<LawyerVerificationRequestResponse>
            {
                Items = dtoItems,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Meta = stats
            };

            return Result.Success(paginatedResult);
        }
    }
}

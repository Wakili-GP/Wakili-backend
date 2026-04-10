using Mapster;
using MediatR;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetVerificationRequests
{
    public class GetLawyerVerificationRequestsQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetLawyerVerificationRequestsQuery, Result<IEnumerable<LawyerVerificationRequestResponse>>>
    {
        public async Task<Result<IEnumerable<LawyerVerificationRequestResponse>>> Handle(
            GetLawyerVerificationRequestsQuery request,
            CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Lawyers.GetVerificationRequestsAsync(
                request.Status,
                cancellationToken);


            return Result.Success(result.Adapt<IEnumerable<LawyerVerificationRequestResponse>>());
        }
    }
}

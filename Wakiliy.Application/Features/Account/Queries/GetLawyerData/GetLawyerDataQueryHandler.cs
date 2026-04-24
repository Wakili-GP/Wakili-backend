using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Queries.GetLawyerData
{
    public class GetLawyerDataQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLawyerDataQuery, Result<LawyerDataDto>>
    {
        public async Task<Result<LawyerDataDto>> Handle(GetLawyerDataQuery request, CancellationToken cancellationToken)
        {
            var lawyer = await unitOfWork.Lawyers.GetByIdAsync(request.LawyerId, cancellationToken);
            if (lawyer is null)
            {
                return Result.Failure<LawyerDataDto>(new Error("Lawyer.NotFound", "Lawyer profile not found", StatusCodes.Status404NotFound));
            }

            var resultDto = lawyer.Adapt<LawyerDataDto>();
            resultDto.MemberSince = lawyer.ApprovedAt;
            return Result.Success(resultDto);
        }
    }
}

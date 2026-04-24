using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetPublicProfileById
{
    public class GetPublicLawyerProfileByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPublicLawyerProfileByIdQuery, Result<PublicLawyerProfileResponseDto>>
    {
        public async Task<Result<PublicLawyerProfileResponseDto>> Handle(GetPublicLawyerProfileByIdQuery request, CancellationToken cancellationToken)
        {
            var dto = await unitOfWork.Lawyers.GetLawyersQueryable()
                .Where(l => l.Id == request.Id)
                .ProjectToType<PublicLawyerProfileResponseDto>()
                .FirstOrDefaultAsync(cancellationToken);

            if (dto is null)
                return Result.Failure<PublicLawyerProfileResponseDto>(new Error("Lawyer.NotFound", "Lawyer not found", StatusCodes.Status404NotFound));

            return Result.Success(dto);
        }
    }
}

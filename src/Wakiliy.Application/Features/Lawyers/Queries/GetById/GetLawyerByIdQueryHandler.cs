using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetById
{
    public class GetLawyerByIdQueryHandler(UserManager<AppUser> userManager) : IRequestHandler<GetLawyerByIdQuery, Result<LawyerDetailsResponse>>
    {
        public async Task<Result<LawyerDetailsResponse>> Handle(GetLawyerByIdQuery request, CancellationToken cancellationToken)
        {
            var dto = await userManager.Users
                .OfType<Lawyer>()
                .AsNoTracking()
                .Where(l => l.Id == request.Id)
                .ProjectToType<LawyerDetailsResponse>()
                .FirstOrDefaultAsync(cancellationToken);

            if (dto is null)
                return Result.Failure<LawyerDetailsResponse>(new Error("Lawyer.NotFound", "Lawyer not found", StatusCodes.Status404NotFound));

            return Result.Success(dto);
        }
    }
}
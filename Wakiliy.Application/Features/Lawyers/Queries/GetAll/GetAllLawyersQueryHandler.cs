using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetAll
{
    public class GetAllLawyersQueryHandler(UserManager<AppUser> userManager) : IRequestHandler<GetAllLawyersQuery, Result<List<LawyerResponse>>>
    {
        public async Task<Result<List<LawyerResponse>>> Handle(GetAllLawyersQuery request, CancellationToken cancellationToken)
        {
            var lawyers = await userManager.Users
                .OfType<Lawyer>()
                .AsNoTracking()
                .ProjectToType<LawyerResponse>()
                .ToListAsync(cancellationToken);

            return Result.Success(lawyers);
        }
    }
}
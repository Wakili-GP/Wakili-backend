using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetApprovedLawyers;

public class GetApprovedLawyersQueryHandler(UserManager<AppUser> userManager)
    : IRequestHandler<GetApprovedLawyersQuery, Result<List<LawyerResponse>>>
{
    public async Task<Result<List<LawyerResponse>>> Handle(GetApprovedLawyersQuery request, CancellationToken cancellationToken)
    {
        var lawyers = await userManager.Users
            .OfType<Lawyer>()
            .Where(l => l.VerificationStatus == VerificationStatus.Approved && l.IsActive)
            .AsNoTracking()
            .ProjectToType<LawyerResponse>()
            .ToListAsync(cancellationToken);

        return Result.Success(lawyers);
    }
}

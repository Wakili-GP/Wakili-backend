using Mapster;
using MediatR;
using Wakiliy.Application.Features.Admins.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Admins.Queries.GetAdmins
{
    public class GetAdminsQueryHandler(IAdminRepository adminRepository) : IRequestHandler<GetAdminsQuery, Result<IEnumerable<AdminDto>>>
    {
        public async Task<Result<IEnumerable<AdminDto>>> Handle(GetAdminsQuery request, CancellationToken cancellationToken)
        {
            var admins = await adminRepository.GetAdminsAsync();

            return Result.Success(admins.Adapt<IEnumerable<AdminDto>>());
        }
    }
}

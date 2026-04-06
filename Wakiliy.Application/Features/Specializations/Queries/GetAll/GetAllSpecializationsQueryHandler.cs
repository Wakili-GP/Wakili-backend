using Mapster;
using MediatR;
using Wakiliy.Application.Features.Specializations.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Specializations.Queries.GetAll;

public class GetAllSpecializationsQueryHandler(ISpecializationRepository specializationRepository)
    : IRequestHandler<GetAllSpecializationsQuery, Result<List<SpecializationResponse>>>
{
    public async Task<Result<List<SpecializationResponse>>> Handle(GetAllSpecializationsQuery request, CancellationToken cancellationToken)
    {
        var items = specializationRepository.GetAll()
        .OrderByDescending(s => s.CreatedOn)
        .ProjectToType<SpecializationResponse>()
        .ToList();
        
        return Result.Success(items);
    }
}

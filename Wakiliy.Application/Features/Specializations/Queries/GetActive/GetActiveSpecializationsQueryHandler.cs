using Mapster;
using MediatR;
using Wakiliy.Application.Features.Specializations.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Specializations.Queries.GetActive;

public class GetActiveSpecializationsQueryHandler(ISpecializationRepository specializationRepository)
    : IRequestHandler<GetActiveSpecializationsQuery, Result<List<SpecializationOptionDto>>>
{
    public async Task<Result<List<SpecializationOptionDto>>> Handle(GetActiveSpecializationsQuery request, CancellationToken cancellationToken)
    {
        var items = await specializationRepository.GetActiveAsync(cancellationToken);
        return Result.Success(items.Adapt<List<SpecializationOptionDto>>());
    }
}

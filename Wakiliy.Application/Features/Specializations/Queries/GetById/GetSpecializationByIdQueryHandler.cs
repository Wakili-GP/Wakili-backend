using Mapster;
using MediatR;
using Wakiliy.Application.Features.Specializations.DTOs;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Specializations.Queries.GetById;

public class GetSpecializationByIdQueryHandler(ISpecializationRepository specializationRepository)
    : IRequestHandler<GetSpecializationByIdQuery, Result<SpecializationResponse>>
{
    public async Task<Result<SpecializationResponse>> Handle(GetSpecializationByIdQuery request, CancellationToken cancellationToken)
    {
        var specialization = await specializationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (specialization is null)
        {
            return Result.Failure<SpecializationResponse>(SpecializationErrors.NotFound);
        }

        return Result.Success(specialization.Adapt<SpecializationResponse>());
    }
}

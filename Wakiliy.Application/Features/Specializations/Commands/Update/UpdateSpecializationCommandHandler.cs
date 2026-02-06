using System;
using Mapster;
using MediatR;
using Wakiliy.Application.Features.Specializations.DTOs;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Specializations.Commands.Update;

public class UpdateSpecializationCommandHandler(ISpecializationRepository specializationRepository)
    : IRequestHandler<UpdateSpecializationCommand, Result<SpecializationResponse>>
{
    public async Task<Result<SpecializationResponse>> Handle(UpdateSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = await specializationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (specialization is null)
            return Result.Failure<SpecializationResponse>(SpecializationErrors.NotFound);

        if (await specializationRepository.ExistsByNameAsync(request.Name, request.Id, cancellationToken))
            return Result.Failure<SpecializationResponse>(SpecializationErrors.DuplicateName);

        request.Adapt(specialization);
        specialization.UpdatedById = request.UserId;
        specialization.UpdatedOn = DateTime.UtcNow;

        await specializationRepository.UpdateAsync(specialization, cancellationToken);

        return Result.Success(specialization.Adapt<SpecializationResponse>());
    }
}

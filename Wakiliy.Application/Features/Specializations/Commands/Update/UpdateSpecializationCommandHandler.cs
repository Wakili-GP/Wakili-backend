using System;
using Mapster;
using MediatR;
using Wakiliy.Application.Features.Specializations.DTOs;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Specializations.Commands.Update;

public class UpdateSpecializationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateSpecializationCommand, Result<SpecializationResponse>>
{
    public async Task<Result<SpecializationResponse>> Handle(UpdateSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = await unitOfWork.Specializations.GetByIdAsync(request.Id, cancellationToken);
        if (specialization is null)
            return Result.Failure<SpecializationResponse>(SpecializationErrors.NotFound);

        if (await unitOfWork.Specializations.ExistsByNameAsync(request.Name, request.Id, cancellationToken))
            return Result.Failure<SpecializationResponse>(SpecializationErrors.DuplicateName);

        request.Adapt(specialization);
        specialization.UpdatedById = request.UserId;
        specialization.UpdatedOn = DateTime.UtcNow;

        await unitOfWork.Specializations.UpdateAsync(specialization, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(specialization.Adapt<SpecializationResponse>());
    }
}

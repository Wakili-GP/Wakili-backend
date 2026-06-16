using System;
using Mapster;
using MediatR;
using Wakiliy.Application.Features.Specializations.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Specializations.Commands.Create;

public class CreateSpecializationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateSpecializationCommand, Result<SpecializationResponse>>
{
    public async Task<Result<SpecializationResponse>> Handle(CreateSpecializationCommand request, CancellationToken cancellationToken)
    {
        if (await unitOfWork.Specializations.ExistsByNameAsync(request.Name, null, cancellationToken))
            return Result.Failure<SpecializationResponse>(SpecializationErrors.DuplicateName);

        var spec = request.Adapt<Specialization>();
        spec.CreatedById = request.UserId;

        await unitOfWork.Specializations.AddAsync(spec, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(spec.Adapt<SpecializationResponse>());
    }
}

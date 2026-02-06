using MediatR;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Specializations.Commands.Delete;

public class DeleteSpecializationCommandHandler(ISpecializationRepository specializationRepository)
    : IRequestHandler<DeleteSpecializationCommand, Result>
{
    public async Task<Result> Handle(DeleteSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = await specializationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (specialization is null)
        {
            return Result.Failure(SpecializationErrors.NotFound);
        }

        await specializationRepository.DeleteAsync(specialization, cancellationToken);
        return Result.Success();
    }
}

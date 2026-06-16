using MediatR;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Specializations.Commands.Delete;

public class DeleteSpecializationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteSpecializationCommand, Result>
{
    public async Task<Result> Handle(DeleteSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = await unitOfWork.Specializations.GetByIdAsync(request.Id, cancellationToken);
        if (specialization is null)
            return Result.Failure(SpecializationErrors.NotFound);

        await unitOfWork.Specializations.DeleteAsync(specialization, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

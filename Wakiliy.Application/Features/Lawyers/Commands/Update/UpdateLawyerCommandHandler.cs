using System.Linq;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.Update
{
    public class UpdateLawyerCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateLawyerCommand, Result<LawyerResponse>>
    {
        public async Task<Result<LawyerResponse>> Handle(UpdateLawyerCommand request, CancellationToken cancellationToken)
        {
            var lawyer = await unitOfWork.Lawyers.GetByIdAsync(request.Id, cancellationToken);

            if (lawyer is null)
                return Result.Failure<LawyerResponse>(new Error("Lawyer.NotFound", "Lawyer not found", StatusCodes.Status404NotFound));

            request.Adapt(lawyer);

            if (request.SpecializationIds is not null)
            {
                var ids = request.SpecializationIds.Distinct().ToList();
                var specializations = await unitOfWork.Specializations.GetByIdsAsync(ids, cancellationToken);
                if (specializations.Count != ids.Count)
                    return Result.Failure<LawyerResponse>(SpecializationErrors.InvalidSelection);

                lawyer.Specializations.Clear();
                foreach (var specialization in specializations)
                    lawyer.Specializations.Add(specialization);
            }

            await unitOfWork.Lawyers.UpdateAsync(lawyer, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success<LawyerResponse>(lawyer.Adapt<LawyerResponse>());
        }
    }
}

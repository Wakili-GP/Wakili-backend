using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Commands.UpdateLawyerInfo
{
    public class UpdateLawyerInfoCommandHandler(ILawyerRepository lawyerRepository,ILogger<UpdateLawyerInfoCommandHandler> logger, ISpecializationRepository specializationRepository) : IRequestHandler<UpdateLawyerInfoCommand, Result<UserInfoResponse>>
    {
        public async Task<Result<UserInfoResponse>> Handle(UpdateLawyerInfoCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("UpdateLawyerInfoCommandHandler: {Id}", request.Id);
            var lawyer = await lawyerRepository.GetByIdAsync(request.Id, cancellationToken);
            if (lawyer is null)
            {
                return Result.Failure<UserInfoResponse>(new Error("Lawyer.NotFound", "Lawyer profile not found or user is not a lawyer", StatusCodes.Status404NotFound));
            }

            request.Adapt(lawyer);

            if (request.SpecializationIds is not null)
            {
                var ids = request.SpecializationIds.Distinct().ToList();
                var specializations = await specializationRepository.GetByIdsAsync(ids, cancellationToken);
                if (specializations.Count != ids.Count)
                {
                    return Result.Failure<UserInfoResponse>(SpecializationErrors.InvalidSelection);
                }

                lawyer.Specializations.Clear();
                foreach (var specialization in specializations)
                {
                    lawyer.Specializations.Add(specialization);
                }
            }

            await lawyerRepository.UpdateAsync(lawyer, cancellationToken);

            return Result.Success(lawyer.Adapt<UserInfoResponse>());
        }
    }
}

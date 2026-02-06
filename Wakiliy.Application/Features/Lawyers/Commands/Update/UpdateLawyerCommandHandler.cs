using System;
using System.Linq;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.Update
{
    public class UpdateLawyerCommandHandler(UserManager<AppUser> userManager, ISpecializationRepository specializationRepository) : IRequestHandler<UpdateLawyerCommand, Result<LawyerResponse>>
    {
        public async Task<Result<LawyerResponse>> Handle(UpdateLawyerCommand request, CancellationToken cancellationToken)
        {
            var lawyer = await userManager.Users
                .OfType<Lawyer>()
                .Include(l => l.Specializations)
                .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

            if (lawyer is null)
            {
                return Result.Failure<LawyerResponse>(new Error("Lawyer.NotFound", "Lawyer not found", StatusCodes.Status404NotFound));
            }

            request.Adapt(lawyer);

           

            if (request.SpecializationIds is not null)
            {
                var ids = request.SpecializationIds.Distinct().ToList();
                var specializations = await specializationRepository.GetByIdsAsync(ids, cancellationToken);
                if (specializations.Count != ids.Count)
                {
                    return Result.Failure<LawyerResponse>(SpecializationErrors.InvalidSelection);
                }

                lawyer.Specializations.Clear();
                foreach (var specialization in specializations)
                {
                    lawyer.Specializations.Add(specialization);
                }
            }

            var result = await userManager.UpdateAsync(lawyer);

            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure<LawyerResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }

            return Result.Success<LawyerResponse>(lawyer.Adapt<LawyerResponse>());
        }
    }
}

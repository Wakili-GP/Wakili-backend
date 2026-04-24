using Mapster;
using MediatR;
using Wakiliy.Application.Common.Models;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Queries.GetByLawyer;

public class GetAppointmentsByLawyerQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAppointmentsByLawyerQuery, Result<PaginatedResult<LawyerReceivedAppointmentDto>>>
{
    public async Task<Result<PaginatedResult<LawyerReceivedAppointmentDto>>> Handle(GetAppointmentsByLawyerQuery request, CancellationToken cancellationToken)
    {
        var (appointments, totalCount,stats) = await unitOfWork.Appointments.GetByLawyerPagedAsync(
            request.LawyerId,
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.Status,
            request.SortDescending,
            cancellationToken
        );

        var dtoItems = appointments.Adapt<List<LawyerReceivedAppointmentDto>>();

        var paginatedResult = new PaginatedResult<LawyerReceivedAppointmentDto>
        {
            Items = dtoItems,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            Meta = stats
        };

        return Result.Success(paginatedResult);
    }
}

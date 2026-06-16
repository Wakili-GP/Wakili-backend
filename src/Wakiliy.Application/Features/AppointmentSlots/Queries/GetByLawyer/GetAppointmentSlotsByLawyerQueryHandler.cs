using Mapster;
using MediatR;
using Wakiliy.Application.Features.AppointmentSlots.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.AppointmentSlots.Queries.GetByLawyer;

public class GetAppointmentSlotsByLawyerQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAppointmentSlotsByLawyerQuery, Result<List<AppointmentSlotDto>>>
{
    public async Task<Result<List<AppointmentSlotDto>>> Handle(GetAppointmentSlotsByLawyerQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.AppointmentSlots.GetByLawyerIdQuery(request.LawyerId, request.Date!.Value,request.SessionType);

        var items = query.Adapt<IEnumerable<AppointmentSlotDto>>().ToList();
        return Result.Success(items);
    }
}
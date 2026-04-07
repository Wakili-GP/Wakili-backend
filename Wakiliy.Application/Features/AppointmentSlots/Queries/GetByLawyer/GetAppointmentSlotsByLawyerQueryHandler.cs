using Mapster;
using MediatR;
using Wakiliy.Application.Features.AppointmentSlots.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.AppointmentSlots.Queries.GetByLawyer;

public class GetAppointmentSlotsByLawyerQueryHandler(IAppointmentSlotRepository appointmentSlotRepository) 
    : IRequestHandler<GetAppointmentSlotsByLawyerQuery, Result<List<AppointmentSlotDto>>>
{
    public async Task<Result<List<AppointmentSlotDto>>> Handle(GetAppointmentSlotsByLawyerQuery request, CancellationToken cancellationToken)
    {
        var items = appointmentSlotRepository
            .GetByLawyerIdQuery(request.LawyerId)
            .Adapt<IEnumerable<AppointmentSlotDto>>().ToList();

        return Result.Success(items);
    }
}
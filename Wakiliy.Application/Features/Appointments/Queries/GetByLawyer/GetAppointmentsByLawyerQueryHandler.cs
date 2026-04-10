using Mapster;
using MediatR;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Queries.GetByLawyer;

public class GetAppointmentsByLawyerQueryHandler(IAppointmentRepository appointmentRepository)
    : IRequestHandler<GetAppointmentsByLawyerQuery, Result<List<LawyerReceivedAppointmentDto>>>
{
    public async Task<Result<List<LawyerReceivedAppointmentDto>>> Handle(GetAppointmentsByLawyerQuery request, CancellationToken cancellationToken)
    {
        var appointments = await appointmentRepository.GetByLawyerIdAsync(request.LawyerId, cancellationToken);

        var result = appointments.Adapt<List<LawyerReceivedAppointmentDto>>();

        return Result.Success(result);
    }
}

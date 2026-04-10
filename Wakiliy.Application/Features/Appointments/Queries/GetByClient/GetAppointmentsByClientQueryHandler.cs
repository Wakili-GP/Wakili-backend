using Mapster;
using MediatR;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Queries.GetByClient;

public class GetAppointmentsByClientQueryHandler(IAppointmentRepository appointmentRepository)
    : IRequestHandler<GetAppointmentsByClientQuery, Result<List<AppointmentDto>>>
{
    public async Task<Result<List<AppointmentDto>>> Handle(GetAppointmentsByClientQuery request, CancellationToken cancellationToken)
    {
        var appointments = await appointmentRepository.GetByClientIdAsync(request.ClientId, cancellationToken);

        var result = appointments.Adapt<List<AppointmentDto>>();

        return Result.Success(result);
    }
}

using Mapster;
using MediatR;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Queries.GetByClient;

public class GetAppointmentsByClientQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAppointmentsByClientQuery, Result<List<AppointmentDto>>>
{
    public async Task<Result<List<AppointmentDto>>> Handle(GetAppointmentsByClientQuery request, CancellationToken cancellationToken)
    {
        var appointments = await unitOfWork.Appointments.GetByClientIdAsync(request.ClientId, cancellationToken);
        return Result.Success(appointments.Adapt<List<AppointmentDto>>());
    }
}

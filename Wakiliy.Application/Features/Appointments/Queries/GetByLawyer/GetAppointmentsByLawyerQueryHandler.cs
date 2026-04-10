using Mapster;
using MediatR;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Queries.GetByLawyer;

public class GetAppointmentsByLawyerQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAppointmentsByLawyerQuery, Result<List<LawyerReceivedAppointmentDto>>>
{
    public async Task<Result<List<LawyerReceivedAppointmentDto>>> Handle(GetAppointmentsByLawyerQuery request, CancellationToken cancellationToken)
    {
        var appointments = await unitOfWork.Appointments.GetByLawyerIdAsync(request.LawyerId, cancellationToken);
        return Result.Success(appointments.Adapt<List<LawyerReceivedAppointmentDto>>());
    }
}

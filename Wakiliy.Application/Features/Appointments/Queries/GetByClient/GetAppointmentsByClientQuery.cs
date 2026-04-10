using MediatR;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Queries.GetByClient;

public class GetAppointmentsByClientQuery : IRequest<Result<List<AppointmentDto>>>
{
    public string ClientId { get; set; } = string.Empty;
}

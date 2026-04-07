using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Commands.Delete;

public class DeleteAppointmentSlotCommand : IRequest<Result>
{
    public int Id { get; set; }
}
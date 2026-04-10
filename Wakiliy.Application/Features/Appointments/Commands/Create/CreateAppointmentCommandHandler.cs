using Mapster;
using MediatR;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Commands.Create;

public class CreateAppointmentCommandHandler(
    IAppointmentRepository appointmentRepository,
    IAppointmentSlotRepository appointmentSlotRepository,
    ILawyerRepository lawyerRepository)
    : IRequestHandler<CreateAppointmentCommand, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var lawyer = await lawyerRepository.GetByIdAsync(request.LawyerId, cancellationToken);
        if (lawyer is null)
            return Result.Failure<AppointmentDto>(AppointmentErrors.LawyerNotFound);

        if (request.ClientId == request.LawyerId)
            return Result.Failure<AppointmentDto>(AppointmentErrors.CannotBookOwnSlot);

        var slot = await appointmentSlotRepository.GetByIdAsync(request.SlotId, cancellationToken);
        if (slot is null)
            return Result.Failure<AppointmentDto>(AppointmentErrors.SlotNotFound);

        var isBooked = await appointmentRepository.IsSlotBookedAsync(request.SlotId, cancellationToken);
        if (isBooked)
            return Result.Failure<AppointmentDto>(AppointmentErrors.SlotAlreadyBooked);

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            SlotId = request.SlotId,
            ClientId = request.ClientId,
            LawyerId = request.LawyerId,
            Status = Domain.Enums.AppointmentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await appointmentRepository.AddAsync(appointment, cancellationToken);
        
        return Result.Success(appointment!.Adapt<AppointmentDto>());
    }
}

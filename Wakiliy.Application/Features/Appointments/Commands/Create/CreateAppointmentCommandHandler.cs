using Mapster;
using MediatR;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Commands.Create;

public class CreateAppointmentCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateAppointmentCommand, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var lawyer = await unitOfWork.Lawyers.GetByIdAsync(request.LawyerId, cancellationToken);
        if (lawyer is null)
            return Result.Failure<AppointmentDto>(AppointmentErrors.LawyerNotFound);

        if (request.ClientId == request.LawyerId)
            return Result.Failure<AppointmentDto>(AppointmentErrors.CannotBookOwnSlot);

        var slot = await unitOfWork.AppointmentSlots.GetByIdAsync(request.SlotId, cancellationToken);
        if (slot is null)
            return Result.Failure<AppointmentDto>(AppointmentErrors.SlotNotFound);

        var isBooked = await unitOfWork.Appointments.IsSlotBookedAsync(request.SlotId, cancellationToken);
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

        await unitOfWork.Appointments.AddAsync(appointment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(appointment.Adapt<AppointmentDto>());
    }
}

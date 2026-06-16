using Mapster;
using MediatR;
using Wakiliy.Application.Features.AppointmentSlots.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.AppointmentSlots.Commands.Create;

public class CreateAppointmentSlotCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateAppointmentSlotCommand, Result<AppointmentSlotDto>>
{
    public async Task<Result<AppointmentSlotDto>> Handle(CreateAppointmentSlotCommand request, CancellationToken cancellationToken)
    {
        var hasOverlap = await unitOfWork.AppointmentSlots.HasOverlappingSlotAsync(
            request.LawyerId, request.Date, request.StartTime, request.EndTime, null, cancellationToken);

        if (hasOverlap)
            return Result.Failure<AppointmentSlotDto>(new Error("AppointmentSlot.Overlap", "The appointment slot overlaps with an existing slot.", 409));

        var appointmentSlot = new AppointmentSlot
        {
            LawyerId = request.LawyerId,
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SessionType = request.SessionType
        };

        await unitOfWork.AppointmentSlots.AddAsync(appointmentSlot, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(appointmentSlot.Adapt<AppointmentSlotDto>());
    }
}
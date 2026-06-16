using FluentValidation;

namespace Wakiliy.Application.Features.AppointmentSlots.Commands.Update;

public class UpdateAppointmentSlotCommandValidator : AbstractValidator<UpdateAppointmentSlotCommand>
{
    public UpdateAppointmentSlotCommandValidator()
    {
        RuleFor(v => v.Id)
            .GreaterThan(0).WithMessage("Valid Id is required.");

        RuleFor(v => v.Date)
            .NotEmpty().WithMessage("Valid Date is required.");

        RuleFor(v => v.StartTime)
            .NotEmpty().WithMessage("Start Time is required.")
            .LessThan(v => v.EndTime).WithMessage("Start Time must be before End Time.");

        RuleFor(v => v.EndTime)
            .NotEmpty().WithMessage("End Time is required.")
            .GreaterThan(v => v.StartTime).WithMessage("End Time must be after Start Time.");

        RuleFor(v => v.SessionType)
            .IsInEnum().WithMessage("Valid Session Type is required.");
    }
}
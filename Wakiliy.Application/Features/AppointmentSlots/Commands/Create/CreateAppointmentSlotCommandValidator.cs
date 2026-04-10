using FluentValidation;

namespace Wakiliy.Application.Features.AppointmentSlots.Commands.Create;

public class CreateAppointmentSlotCommandValidator : AbstractValidator<CreateAppointmentSlotCommand>
{
    public CreateAppointmentSlotCommandValidator()
    {
        RuleFor(v => v.LawyerId)
            .NotEmpty().WithMessage("Lawyer ID is required.");

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
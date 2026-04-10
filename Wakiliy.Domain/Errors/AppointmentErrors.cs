using Microsoft.AspNetCore.Http;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Domain.Errors;

public static class AppointmentErrors
{
    public static readonly Error SlotNotFound = new("Appointment.SlotNotFound", "The selected appointment slot was not found.", StatusCodes.Status404NotFound);
    public static readonly Error SlotAlreadyBooked = new("Appointment.SlotAlreadyBooked", "The selected appointment slot is already booked.", StatusCodes.Status409Conflict);
    public static readonly Error AppointmentNotFound = new("Appointment.NotFound", "The appointment was not found.", StatusCodes.Status404NotFound);
    public static readonly Error LawyerNotFound = new("Appointment.LawyerNotFound", "The specified lawyer was not found.", StatusCodes.Status404NotFound);
    public static readonly Error CannotBookOwnSlot = new("Appointment.CannotBookOwnSlot", "You cannot book an appointment with yourself.", StatusCodes.Status400BadRequest);
    public static readonly Error InvalidStatusTransition = new("Appointment.InvalidStatus", "The appointment cannot be updated to the requested status.", StatusCodes.Status400BadRequest);
    public static readonly Error Unauthorized = new("Appointment.Unauthorized", "You are not authorized to update this appointment.", StatusCodes.Status403Forbidden);
}

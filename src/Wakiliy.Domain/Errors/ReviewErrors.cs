using Microsoft.AspNetCore.Http;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Domain.Errors;

public static class ReviewErrors
{
    public static readonly Error AppointmentNotFound =
        new("Review.AppointmentNotFound", "The specified appointment was not found.", StatusCodes.Status404NotFound);

    public static readonly Error AppointmentNotCompleted =
        new("Review.AppointmentNotCompleted", "You can only review after the appointment is completed.", StatusCodes.Status400BadRequest);

    public static readonly Error ReviewAlreadyExists =
        new("Review.AlreadyExists", "A review already exists for this appointment.", StatusCodes.Status409Conflict);

    public static readonly Error InvalidRating =
        new("Review.InvalidRating", "Rating must be between 1 and 5, in 0.5 increments.", StatusCodes.Status400BadRequest);
}

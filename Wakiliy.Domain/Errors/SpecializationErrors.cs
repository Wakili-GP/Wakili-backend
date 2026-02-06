using Microsoft.AspNetCore.Http;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Domain.Errors;

public static class SpecializationErrors
{
    public static readonly Error NotFound = new("Specialization.NotFound", "Specialization not found.", StatusCodes.Status404NotFound);
    public static readonly Error DuplicateName = new("Specialization.DuplicateName", "Another specialization with the same name already exists.", StatusCodes.Status409Conflict);
    public static readonly Error InvalidSelection = new("Specialization.InvalidSelection", "One or more selected specializations are invalid or inactive.", StatusCodes.Status400BadRequest);
}

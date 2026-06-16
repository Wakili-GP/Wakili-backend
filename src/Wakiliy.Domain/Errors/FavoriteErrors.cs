using Microsoft.AspNetCore.Http;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Domain.Errors;

public static class FavoriteErrors
{
    public static readonly Error LawyerNotFound = new("Favorite.LawyerNotFound", "Lawyer not found.", StatusCodes.Status404NotFound);
    public static readonly Error AlreadyFavorited = new("Favorite.AlreadyFavorited", "This lawyer is already in your favorites.", StatusCodes.Status409Conflict);
    public static readonly Error NotInFavorites = new("Favorite.NotInFavorites", "This lawyer is not in your favorites.", StatusCodes.Status404NotFound);
}

namespace Wakiliy.Domain.Entities;
public class Client : AppUser
{
    public string NationalId { get; set; } = string.Empty;
    public ICollection<FavoriteLawyer> FavoriteLawyers { get; set; } = new List<FavoriteLawyer>();
}

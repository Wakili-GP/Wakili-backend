namespace Wakiliy.Domain.Entities;

public class FavoriteLawyer
{
    public string UserId { get; set; } = string.Empty;
    public Client User { get; set; } = null!;

    public string LawyerId { get; set; } = string.Empty;
    public Lawyer Lawyer { get; set; } = null!;
}

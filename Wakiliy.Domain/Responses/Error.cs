namespace Wakiliy.Domain.Responses;
public record Error(string code, string Description, int? statusCode)
{
    public static readonly Error None = new(string.Empty, string.Empty, null);
}

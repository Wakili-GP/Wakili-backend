namespace Wakiliy.Domain.Entities;

public class PaymentTransaction
{
    public Guid Id { get; set; }
    public Guid BookingIntentId { get; set; }
    public decimal Amount { get; set; }
    public string Provider { get; set; } = "Paymob";
    public string Status { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string RawResponse { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public BookingIntent BookingIntent { get; set; } = default!;
}
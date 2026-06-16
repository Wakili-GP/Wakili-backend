namespace Wakiliy.Application.Interfaces.Services;

public interface IPaymobService
{
    Task<string> CreatePaymentKeyAsync(decimal amount, string billingEmail, string billingFirstName, string billingLastName, string billingPhone, string orderId);
    bool VerifyHmac(string hmac, Dictionary<string, string> properties);
}
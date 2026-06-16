using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Wakiliy.Application.Common.Settings;
using Wakiliy.Application.Interfaces.Services;

namespace Wakiliy.Infrastructure.Services;

public class PaymobService(HttpClient httpClient, IOptions<PaymobSettings> settings) : IPaymobService
{
    private readonly PaymobSettings _settings = settings.Value;

    public async Task<string> CreatePaymentKeyAsync(decimal amount, string billingEmail, string billingFirstName, string billingLastName, string billingPhone, string orderId)
    {
        // 1. Authenticate
        var authResponse = await httpClient.PostAsJsonAsync("auth/tokens", new { api_key = _settings.ApiKey });
        authResponse.EnsureSuccessStatusCode();
        var authResult = await authResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = authResult.GetProperty("token").GetString();

        // 2. Order Registration
        var amountCents = (int)(amount * 100);
        var orderResponse = await httpClient.PostAsJsonAsync("ecommerce/orders", new
        {
            auth_token = token,
            delivery_needed = "false",
            amount_cents = amountCents,
            currency = "EGP",
            merchant_order_id = orderId
        });
        orderResponse.EnsureSuccessStatusCode();
        var orderResult = await orderResponse.Content.ReadFromJsonAsync<JsonElement>();
        var paymobOrderId = orderResult.GetProperty("id").GetInt32().ToString();

        // 3. Payment Key Generation
        var paymentKeyResponse = await httpClient.PostAsJsonAsync("acceptance/payment_keys", new
        {
            auth_token = token,
            amount_cents = amountCents,
            expiration = 3600,
            order_id = paymobOrderId,
            billing_data = new {
                apartment = "NA", 
                email = string.IsNullOrWhiteSpace(billingEmail) ? "client@wakiliy.com" : billingEmail, 
                floor = "NA", 
                first_name = string.IsNullOrWhiteSpace(billingFirstName) ? "Client" : billingFirstName,
                street = "NA", 
                building = "NA", 
                phone_number = string.IsNullOrWhiteSpace(billingPhone) ? "01000000000" : billingPhone, 
                shipping_method = "NA",
                postal_code = "NA", 
                city = "NA", 
                country = "EG", 
                last_name = string.IsNullOrWhiteSpace(billingLastName) ? "User" : billingLastName,
                state = "NA"
            },
            currency = "EGP",
            integration_id = _settings.IntegrationId
        });
        
        paymentKeyResponse.EnsureSuccessStatusCode();
        var paymentKeyResult = await paymentKeyResponse.Content.ReadFromJsonAsync<JsonElement>();
        return paymentKeyResult.GetProperty("token").GetString() ?? throw new Exception("Failed to get Paymob token");
    }

    public bool VerifyHmac(string hmac, Dictionary<string, string> properties)
    {
        // Paymob requires specific keys concatenated in an exact lexicographical order
        var keys = new[] { 
            "amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction", 
            "id", "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_refunded", 
            "is_standalone_payment", "is_voided", "order.id", "owner", "pending", 
            "source_data.pan", "source_data.sub_type", "source_data.type", "success" 
        };
        
        var concatenatedStr = string.Join("", keys.Select(k => 
        {
            if (properties.TryGetValue(k, out var val))
                return val;
                
            // Fallback for GET webhook where order is sent as 'order' and not 'order.id'
            if (k == "order.id" && properties.TryGetValue("order", out var valOrd))
                return valOrd;

            return "";
        }));
        
        using var hmacSha512 = new HMACSHA512(Encoding.UTF8.GetBytes(_settings.HmacSecret));
        var hash = hmacSha512.ComputeHash(Encoding.UTF8.GetBytes(concatenatedStr));
        var calculatedHmac = BitConverter.ToString(hash).Replace("-", "").ToLower();
        
        return calculatedHmac == hmac.ToLower();
    }
}
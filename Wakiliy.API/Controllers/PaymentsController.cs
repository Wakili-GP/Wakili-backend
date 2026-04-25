using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.Application.Features.Payments.Commands.HandlePaymobWebhook;

namespace Wakiliy.API.Controllers;

[Route("api/payments")]
[ApiController]
public class PaymentsController(IMediator mediator, ILogger<PaymentsController> logger) : ControllerBase
{
    [HttpGet("webhook")]
    public async Task<IActionResult> Webhook([FromQuery] string hmac, [FromQuery] Dictionary<string, string> queryParameters)
    {
        var queryString = HttpContext.Request.QueryString.ToString();
        logger.LogInformation("Received Paymob Webhook GET request: {QueryString}", queryString);

        try
        {
            // Remove HMAC from properties for processing if it was captured as part of query string logic
            var properties = queryParameters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            
            // Map the merchant_order_id query param to Paymob's default object payload key used in the backend
            if (properties.TryGetValue("merchant_order_id", out var merchantOrderId))
            {
               properties["order.merchant_order_id"] = merchantOrderId;
            }

            var command = new HandlePaymobWebhookCommand
            {
                Hmac = hmac,
                Payload = properties
            };

            await mediator.Send(command);

            logger.LogInformation("Successfully processed Paymob Webhook for Order ID: {OrderId}", merchantOrderId ?? "N/A");

            return Redirect("https://wakiliy.com/payment/success"); // Redirect back to frontend on successful validation
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing Paymob Webhook GET url.");
            return Redirect("https://wakiliy.com/payment/failed"); 
        }
    }
    
    [HttpPost("webhook")]
    public async Task<IActionResult> WebhookPost([FromQuery] string hmac, [FromBody] JsonElement payload)
    {
      logger.LogInformation("Received Paymob Webhook: {Payload}", payload.ToString());
        try
        {
            // Convert Webhook payload elements to flat key-value dictionary as required for HMAC calculation
            var properties = new Dictionary<string, string>();
            var obj = payload.GetProperty("obj");

            void FlattenJson(JsonElement element, string prefix)
            {
                if (element.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in element.EnumerateObject())
                    {
                        var key = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";
                        FlattenJson(prop.Value, key);
                    }
                }
                else if (element.ValueKind == JsonValueKind.True)
                {
                    properties[prefix] = "true";
                }
                else if (element.ValueKind == JsonValueKind.False)
                {
                    properties[prefix] = "false";
                }
                else
                {
                    properties[prefix] = element.ToString();
                }
            }

            FlattenJson(obj, "");

            logger.LogInformation("Flattened Webhook Properties: {Properties}", properties);

            var command = new HandlePaymobWebhookCommand
            {
                Hmac = hmac,
                Payload = properties
            };

            await mediator.Send(command);

            logger.LogInformation("Successfully processed Paymob Webhook for Order ID: {OrderId}", properties.GetValueOrDefault("order.merchant_order_id", "N/A"));

            return Ok(); // ALWAYS return OK to avoid retries
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing Paymob Webhook.");
            return Ok(); // STILL return OK even on exceptions
        }
    }
}
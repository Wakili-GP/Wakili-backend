using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Wakiliy.Application.Interfaces.Services;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Payments.Commands.HandlePaymobWebhook;

public class HandlePaymobWebhookCommand : IRequest<Result>
{
    public string Hmac { get; set; } = string.Empty;
    public Dictionary<string, string> Payload { get; set; } = new();
}

public class HandlePaymobWebhookCommandHandler(
    IUnitOfWork unitOfWork,
    IPaymobService paymobService,
    ILogger<HandlePaymobWebhookCommandHandler> logger)
    : IRequestHandler<HandlePaymobWebhookCommand, Result>
{
    public async Task<Result> Handle(HandlePaymobWebhookCommand request, CancellationToken cancellationToken)
    {
        // 🔐 1. Verify HMAC
        if (!paymobService.VerifyHmac(request.Hmac, request.Payload))
        {
            logger.LogWarning("Invalid HMAC");
            return Result.Success(); // ignore silently
        }

        // 📌 2. Get Order Id
        var orderIdStr =
            request.Payload.GetValueOrDefault("merchant_order_id") ??
            request.Payload.GetValueOrDefault("order.merchant_order_id");

        if (string.IsNullOrEmpty(orderIdStr) || !Guid.TryParse(orderIdStr, out var intentId))
        {
            logger.LogWarning("Invalid or missing merchant_order_id");
            return Result.Success();
        }

        // 📦 3. Get intent
        var intent = await unitOfWork.BookingIntents.GetByIdAsync(intentId, cancellationToken);

        if (intent == null || intent.Status != BookingIntentStatus.Pending)
        {
            logger.LogInformation("Already processed or not found");
            return Result.Success();
        }

        // 💰 4. Parse Amount
        decimal amount = decimal.Parse(request.Payload.GetValueOrDefault("amount_cents", "0")) / 100;

        // ✅ 5. Determine Success (IMPORTANT FIX)
        var successParsed = bool.TryParse(request.Payload.GetValueOrDefault("success", "false"), out var successVal) && successVal;

        var txnCode =
            request.Payload.GetValueOrDefault("txn_response_code") ??
            request.Payload.GetValueOrDefault("data.txn_response_code");

        var isSuccess = successParsed && txnCode == "APPROVED";

        // 💾 6. Save Transaction
        var transaction = new PaymentTransaction
        {
            Id = Guid.NewGuid(),
            BookingIntentId = intentId,
            Amount = amount,
            Status = isSuccess ? "Success" : "Failed",
            TransactionId = request.Payload.GetValueOrDefault("id", ""),
            RawResponse = string.Join(";", request.Payload.Select(kv => $"{kv.Key}={kv.Value}"))
        };

        await unitOfWork.PaymentTransactions.AddAsync(transaction, cancellationToken);

        // 🎯 7. Update Intent
        if (isSuccess)
        {
            intent.Status = BookingIntentStatus.Paid;

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                SlotId = intent.SlotId,
                ClientId = intent.ClientId,
                LawyerId = intent.LawyerId,
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.Appointments.AddAsync(appointment, cancellationToken);

            logger.LogInformation("Payment SUCCESS for {OrderId}", intentId);
        }
        else
        {
            intent.Status = BookingIntentStatus.Failed;
            logger.LogWarning("Payment FAILED for {OrderId}", intentId);
        }

        unitOfWork.BookingIntents.Update(intent);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
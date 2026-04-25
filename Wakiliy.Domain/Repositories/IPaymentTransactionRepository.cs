using Wakiliy.Domain.Entities;

namespace Wakiliy.Domain.Repositories;

public interface IPaymentTransactionRepository
{
    Task AddAsync(PaymentTransaction paymentTransaction, CancellationToken cancellationToken = default);
}
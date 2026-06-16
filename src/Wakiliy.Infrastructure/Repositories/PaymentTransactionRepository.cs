using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories;

public class PaymentTransactionRepository(ApplicationDbContext dbContext) : IPaymentTransactionRepository
{
    public async Task AddAsync(PaymentTransaction paymentTransaction, CancellationToken cancellationToken = default)
    {
        await dbContext.PaymentTransactions.AddAsync(paymentTransaction, cancellationToken);
    }
}
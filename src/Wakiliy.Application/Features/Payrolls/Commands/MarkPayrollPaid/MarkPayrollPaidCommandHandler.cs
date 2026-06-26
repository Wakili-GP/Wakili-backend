using MediatR;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Payrolls.Commands.MarkPayrollPaid;

public class MarkPayrollPaidCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<MarkPayrollPaidCommand, Result>
{
    public async Task<Result> Handle(MarkPayrollPaidCommand request, CancellationToken cancellationToken)
    {
        var payroll = await unitOfWork.Payrolls.GetByIdWithEarningsAsync(request.PayrollId, cancellationToken);

        if (payroll == null)
            return Result.Failure(new Error("NotFound", "Payroll not found.", 404));

        if (payroll.Status == PayrollStatus.Paid)
            return Result.Failure(new Error("InvalidStatus", "Payroll is already paid.", 400));

        payroll.Status = PayrollStatus.Paid;
        payroll.PaidAt = DateTime.UtcNow;

        foreach (var earning in payroll.Earnings)
        {
            earning.Status = LawyerEarningStatus.Paid;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

using MediatR;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Payrolls.Commands.MarkPayrollFailed;

public class MarkPayrollFailedCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<MarkPayrollFailedCommand, Result>
{
    public async Task<Result> Handle(MarkPayrollFailedCommand request, CancellationToken cancellationToken)
    {
        var payroll = await unitOfWork.Payrolls.GetByIdAsync(request.PayrollId, cancellationToken);

        if (payroll == null)
            return Result.Failure(new Error("NotFound", "Payroll not found.", 404));

        if (payroll.Status != PayrollStatus.Pending)
            return Result.Failure(new Error("InvalidStatus", "Payroll can only be marked as failed if pending.", 400));

        payroll.Status = PayrollStatus.Failed;

        // Earnings remain pending so they can be included in a future payroll
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

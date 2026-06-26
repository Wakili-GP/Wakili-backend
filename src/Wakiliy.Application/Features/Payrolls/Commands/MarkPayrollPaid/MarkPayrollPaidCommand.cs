using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Payrolls.Commands.MarkPayrollPaid;

public class MarkPayrollPaidCommand : IRequest<Result>
{
    public int PayrollId { get; set; }
}

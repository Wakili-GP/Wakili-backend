using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Payrolls.Commands.MarkPayrollFailed;

public class MarkPayrollFailedCommand : IRequest<Result>
{
    public int PayrollId { get; set; }
}

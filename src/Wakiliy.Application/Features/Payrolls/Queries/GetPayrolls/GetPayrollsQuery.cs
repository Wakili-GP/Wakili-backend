using MediatR;
using Wakiliy.Application.Features.Payrolls.DTOs;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Payrolls.Queries.GetPayrolls;

public class GetPayrollsQuery : IRequest<Result<List<PayrollDto>>>
{
    public string? LawyerId { get; set; }
    public PayrollStatus? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? Search { get; set; }
}

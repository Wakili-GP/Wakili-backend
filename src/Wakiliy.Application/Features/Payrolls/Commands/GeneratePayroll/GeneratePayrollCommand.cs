using MediatR;
using Wakiliy.Application.Features.Payrolls.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Payrolls.Commands.GeneratePayroll;

public class GeneratePayrollCommand : IRequest<Result<PayrollDetailsDto>>
{
    public string LawyerId { get; set; } = string.Empty;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

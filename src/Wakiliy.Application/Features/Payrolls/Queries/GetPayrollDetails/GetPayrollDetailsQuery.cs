using MediatR;
using Wakiliy.Application.Features.Payrolls.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Payrolls.Queries.GetPayrollDetails;

public class GetPayrollDetailsQuery : IRequest<Result<PayrollDetailsDto>>
{
    public int PayrollId { get; set; }
}

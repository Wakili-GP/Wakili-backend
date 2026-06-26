using MediatR;
using Wakiliy.Application.Features.Payrolls.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Payrolls.Queries.GetPayrolls;

public class GetPayrollsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPayrollsQuery, Result<List<PayrollDto>>>
{
    public async Task<Result<List<PayrollDto>>> Handle(GetPayrollsQuery request, CancellationToken cancellationToken)
    {
        var payrolls = await unitOfWork.Payrolls.GetAllPayrollsAsync(
            request.LawyerId,
            request.Status,
            request.DateFrom,
            request.DateTo,
            request.Search,
            cancellationToken);

        var result = payrolls.Select(p => new PayrollDto
        {
            Id = p.Id,
            LawyerId = p.LawyerId,
            LawyerName = p.Lawyer.FirstName + " " + p.Lawyer.LastName,
            TotalAmount = p.TotalAmount,
            Status = p.Status.ToString(),
            CreatedAt = p.CreatedAt,
            PaidAt = p.PaidAt
        }).ToList();

        return Result.Success(result);
    }
}

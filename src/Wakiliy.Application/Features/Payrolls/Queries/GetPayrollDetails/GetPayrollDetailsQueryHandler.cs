using MediatR;
using Wakiliy.Application.Features.Earnings.DTOs;
using Wakiliy.Application.Features.Payrolls.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Payrolls.Queries.GetPayrollDetails;

public class GetPayrollDetailsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPayrollDetailsQuery, Result<PayrollDetailsDto>>
{
    public async Task<Result<PayrollDetailsDto>> Handle(GetPayrollDetailsQuery request, CancellationToken cancellationToken)
    {
        var payroll = await unitOfWork.Payrolls.GetByIdWithEarningsAsync(request.PayrollId, cancellationToken);

        if (payroll == null)
            return Result.Failure<PayrollDetailsDto>(new Error("NotFound", "Payroll not found.", 404));

        var details = new PayrollDetailsDto
        {
            Id = payroll.Id,
            LawyerId = payroll.LawyerId,
            LawyerName = payroll.Lawyer.FirstName + " " + payroll.Lawyer.LastName,
            TotalAmount = payroll.TotalAmount,
            Status = payroll.Status.ToString(),
            CreatedAt = payroll.CreatedAt,
            PaidAt = payroll.PaidAt,
            Earnings = payroll.Earnings.Select(e => new EarningDto
            {
                Id = e.Id,
                AppointmentId = e.AppointmentId,
                LawyerId = e.LawyerId,
                LawyerName = payroll.Lawyer.FirstName + " " + payroll.Lawyer.LastName,
                ClientName = e.Appointment.Client.FirstName + " " + e.Appointment.Client.LastName,
                GrossAmount = e.GrossAmount,
                PlatformFee = e.PlatformFee,
                NetAmount = e.NetAmount,
                Status = e.Status.ToString(),
                CreatedAt = e.CreatedAt,
                PayrollId = e.PayrollId
            }).ToList()
        };

        return Result.Success(details);
    }
}

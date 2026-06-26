using MediatR;
using Wakiliy.Application.Features.Earnings.DTOs;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Earnings.Queries.GetLawyerEarnings;

public class GetLawyerEarningsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLawyerEarningsQuery, Result<LawyerEarningsSummaryDto>>
{
    public async Task<Result<LawyerEarningsSummaryDto>> Handle(GetLawyerEarningsQuery request, CancellationToken cancellationToken)
    {
        var earnings = await unitOfWork.LawyerEarnings.GetLawyerEarningsAsync(request.LawyerId, cancellationToken);

        var pendingBalance = earnings.Where(e => e.Status == LawyerEarningStatus.Pending).Sum(e => e.NetAmount);
        var paidBalance = earnings.Where(e => e.Status == LawyerEarningStatus.Paid).Sum(e => e.NetAmount);
        
        var summary = new LawyerEarningsSummaryDto
        {
            AvailableBalance = pendingBalance, // Assuming pending means available for payroll
            PendingBalance = pendingBalance,
            PaidBalance = paidBalance,
            TotalEarnings = pendingBalance + paidBalance,
            Earnings = earnings.Select(e => new EarningDto
            {
                Id = e.Id,
                AppointmentId = e.AppointmentId,
                LawyerId = e.LawyerId,
                ClientName = e.Appointment.Client.FirstName + " " + e.Appointment.Client.LastName,
                GrossAmount = e.GrossAmount,
                PlatformFee = e.PlatformFee,
                NetAmount = e.NetAmount,
                Status = e.Status.ToString(),
                CreatedAt = e.CreatedAt,
                PayrollId = e.PayrollId
            }).ToList()
        };

        return Result.Success(summary);
    }
}

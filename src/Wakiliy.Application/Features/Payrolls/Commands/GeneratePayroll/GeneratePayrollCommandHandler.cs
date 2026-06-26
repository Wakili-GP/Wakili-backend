using MediatR;
using Wakiliy.Application.Features.Earnings.DTOs;
using Wakiliy.Application.Features.Payrolls.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Payrolls.Commands.GeneratePayroll;

public class GeneratePayrollCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<GeneratePayrollCommand, Result<PayrollDetailsDto>>
{
    public async Task<Result<PayrollDetailsDto>> Handle(GeneratePayrollCommand request, CancellationToken cancellationToken)
    {
        var pendingEarnings = await unitOfWork.LawyerEarnings.GetPendingEarningsAsync(request.LawyerId, request.FromDate, request.ToDate, cancellationToken);

        if (!pendingEarnings.Any())
            return Result.Failure<PayrollDetailsDto>(new Error("NoEarnings", "No pending earnings found for the selected period.", 400));

        var totalAmount = pendingEarnings.Sum(e => e.NetAmount);
        var lawyerName = pendingEarnings.First().Lawyer.FirstName + " " + pendingEarnings.First().Lawyer.LastName;

        var payroll = new Payroll
        {
            LawyerId = request.LawyerId,
            TotalAmount = totalAmount,
            Status = PayrollStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Earnings = pendingEarnings
        };

        await unitOfWork.Payrolls.AddAsync(payroll, cancellationToken);

        // Earnings automatically updated via navigation property when added to DbContext tracking
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var details = new PayrollDetailsDto
        {
            Id = payroll.Id,
            LawyerId = payroll.LawyerId,
            LawyerName = lawyerName,
            TotalAmount = payroll.TotalAmount,
            Status = payroll.Status.ToString(),
            CreatedAt = payroll.CreatedAt,
            Earnings = pendingEarnings.Select(e => new EarningDto
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
                PayrollId = payroll.Id
            }).ToList()
        };

        return Result.Success(details);
    }
}

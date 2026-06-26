using MediatR;
using Wakiliy.Application.Features.Earnings.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Earnings.Queries.GetAll;

public class GetAllEarningsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllEarningsQuery, Result<List<EarningDto>>>
{
    public async Task<Result<List<EarningDto>>> Handle(GetAllEarningsQuery request, CancellationToken cancellationToken)
    {
        var earnings = await unitOfWork.LawyerEarnings.GetAllEarningsAsync(
            request.LawyerId,
            request.Status,
            request.DateFrom,
            request.DateTo,
            request.Search,
            cancellationToken);

        var result = earnings.Select(e => new EarningDto
        {
            Id = e.Id,
            AppointmentId = e.AppointmentId,
            LawyerId = e.LawyerId,
            LawyerName = e.Lawyer.FirstName + " " + e.Lawyer.LastName,
            ClientName = e.Appointment.Client.FirstName + " " + e.Appointment.Client.LastName,
            GrossAmount = e.GrossAmount,
            PlatformFee = e.PlatformFee,
            NetAmount = e.NetAmount,
            Status = e.Status.ToString(),
            CreatedAt = e.CreatedAt,
            PayrollId = e.PayrollId
        }).ToList();

        return Result.Success(result);
    }
}

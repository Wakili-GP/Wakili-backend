using Mapster;
using MediatR;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Queries.GetApprovedAppointments;

public class GetApprovedAppointmentsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetApprovedAppointmentsQuery, Result<List<ApprovedAppointmentDto>>>
{
    public async Task<Result<List<ApprovedAppointmentDto>>> Handle(GetApprovedAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var targetDate = DateTime.UtcNow;
        DateTime startDate;
        DateTime endDate;

        switch (request.ViewType)
        {
            case CalendarViewType.Month:
                // Beginning and end of the target month
                startDate = new DateTime(targetDate.Year, targetDate.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
                break;

            case CalendarViewType.Week:
                int diff = (7 + (targetDate.DayOfWeek - DayOfWeek.Saturday)) % 7;
                startDate = targetDate.AddDays(-diff).Date;
                endDate = startDate.AddDays(6);
                break;

            case CalendarViewType.Day:
            default:
                startDate = targetDate.Date;
                endDate = targetDate.Date;
                break;
        }

        var appointments = await unitOfWork.Appointments.GetApprovedAppointmentsAsync(
            request.LawyerId,
            startDate,
            endDate,
            cancellationToken
        );

        return Result.Success(appointments.Adapt<List<ApprovedAppointmentDto>>());
    }
}

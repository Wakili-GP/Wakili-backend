using MediatR;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Reviews.Queries.GetByAppointment;

public class GetReviewByAppointmentIdQuery : IRequest<Result<ReviewResponseDto>>
{
    public Guid AppointmentId { get; set; }
}

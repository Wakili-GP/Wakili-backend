using MediatR;
using Wakiliy.Application.Features.Earnings.DTOs;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Earnings.Queries.GetAll;

public class GetAllEarningsQuery : IRequest<Result<List<EarningDto>>>
{
    public string? LawyerId { get; set; }
    public LawyerEarningStatus? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? Search { get; set; }
    // Pagination could be added here
}

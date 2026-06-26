using MediatR;
using Wakiliy.Application.Features.Earnings.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Earnings.Queries.GetLawyerEarnings;

public class GetLawyerEarningsQuery : IRequest<Result<LawyerEarningsSummaryDto>>
{
    public string LawyerId { get; set; } = string.Empty;
}

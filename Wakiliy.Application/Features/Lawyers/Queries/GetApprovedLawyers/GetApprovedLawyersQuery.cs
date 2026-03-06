using MediatR;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetApprovedLawyers;

public class GetApprovedLawyersQuery : IRequest<Result<List<LawyerResponse>>>
{
}

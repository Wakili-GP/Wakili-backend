using System.Collections.Generic;
using MediatR;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetVerificationRequests
{
    public record GetLawyerVerificationRequestsQuery(VerificationStatus? Status) : IRequest<Result<List<LawyerVerificationRequestResponse>>>;
}

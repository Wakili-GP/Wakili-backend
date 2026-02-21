using MediatR;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetVerificationRequestById
{
    public record GetLawyerVerificationRequestByIdQuery(string Id) : IRequest<Result<LawyerVerificationDetailResponse>>;
}

using MediatR;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetPublicProfileById
{
    public class GetPublicLawyerProfileByIdQuery(string id) : IRequest<Result<PublicLawyerProfileResponseDto>>
    {
        public string Id { get; } = id;
    }
}

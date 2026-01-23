using MediatR;
using System.Text.Json.Serialization;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetById
{
    public class GetLawyerByIdQuery(string id) : IRequest<Result<LawyerDetailsResponse>>
    {
        [JsonIgnore]
        public string Id { get; } = id;
    }
}
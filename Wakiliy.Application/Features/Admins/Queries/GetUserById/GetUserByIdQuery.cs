using MediatR;
using System.Text.Json.Serialization;
using Wakiliy.Application.Features.Admins.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Admins.Queries.GetUserById
{
    public class GetUserByIdQuery : IRequest<Result<UserListItemDto>>
    {
        [JsonIgnore]
        public string Id { get; }

        public GetUserByIdQuery(string id)
        {
            Id = id;
        }
    }
}

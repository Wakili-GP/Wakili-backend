using MediatR;
using System.Text.Json.Serialization;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Queries.GetInfo
{
    public class GetAccountInfoQuery(string id) : IRequest<Result<UserInfoResponse>>
    {
        [JsonIgnore]
        public string Id { get; } = id;
    }
}
using MediatR;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Queries.GetClientData
{
    public class GetClientDataQuery : IRequest<Result<ClientDataDto>>
    {
        public string ClientId { get; set; }

        public GetClientDataQuery(string clientId)
        {
            ClientId = clientId;
        }
    }
}

using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Queries.GetClientData
{
    public class GetClientDataQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetClientDataQuery, Result<ClientDataDto>>
    {

        public async Task<Result<ClientDataDto>> Handle(GetClientDataQuery request, CancellationToken cancellationToken)
        {
            var client = await unitOfWork.Clients.GetByIdAsync(request.ClientId, cancellationToken);
            if (client is null)
            {
                return Result.Failure<ClientDataDto>(new Error("Client.NotFound", "Client profile not found", StatusCodes.Status404NotFound));
            }

            var resultDto = client.Adapt<ClientDataDto>();
            return Result.Success(resultDto);
        }
    }
}

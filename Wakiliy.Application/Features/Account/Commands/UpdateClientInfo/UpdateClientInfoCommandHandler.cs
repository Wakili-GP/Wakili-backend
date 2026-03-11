using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Commands.UpdateClientInfo
{
    public class UpdateClientInfoCommandHandler(IClientRepository clientRepository) : IRequestHandler<UpdateClientInfoCommand, Result<UserInfoResponse>>
    {
        public async Task<Result<UserInfoResponse>> Handle(UpdateClientInfoCommand request, CancellationToken cancellationToken)
        {
            var client = await clientRepository.GetByIdAsync(request.Id, cancellationToken);
            if (client is null)
            {
                return Result.Failure<UserInfoResponse>(new Error("Client.NotFound", "Client profile not found or user is not a client", StatusCodes.Status404NotFound));
            }

            request.Adapt(client);

            await clientRepository.UpdateAsync(client, cancellationToken);

            return Result.Success(client.Adapt<UserInfoResponse>());
        }
    }
}

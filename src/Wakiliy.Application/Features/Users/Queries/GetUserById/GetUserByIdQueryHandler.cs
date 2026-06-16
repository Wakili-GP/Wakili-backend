using Mapster;
using MediatR;
using Wakiliy.Application.Features.Users.DTOs;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetUserByIdQuery, Result<UserListItemDto>>
    {
        public async Task<Result<UserListItemDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.GetUserByIdAsync(request.Id);

            if (user is null)
                return Result.Failure<UserListItemDto>(UserErrors.UserNotFound);

            return Result.Success(user.Adapt<UserListItemDto>());
        }
    }
}

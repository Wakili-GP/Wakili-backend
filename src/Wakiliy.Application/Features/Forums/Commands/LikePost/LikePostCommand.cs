using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Forums.Commands.LikePost;

public record LikePostCommand(
    string PostId
) : IRequest<Result<string>>;

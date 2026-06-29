using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Forums.Commands.LikeComment;

public record LikeCommentCommand(
    string CommentId
) : IRequest<Result<string>>;

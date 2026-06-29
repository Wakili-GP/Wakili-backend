using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;
using Wakiliy.Domain.Errors;

namespace Wakiliy.Application.Features.Forums.Commands.LikeComment;

public class LikeCommentCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<LikeCommentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LikeCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await unitOfWork.Forums.GetCommentByIdAsync(request.CommentId, cancellationToken);
        if (comment == null)
            return Result.Failure<string>(new Error("CommentNotFound", "The forum comment was not found.", 404));

        comment.LikesCount++;
        unitOfWork.Forums.UpdateComment(comment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Comment liked successfully.");
    }
}

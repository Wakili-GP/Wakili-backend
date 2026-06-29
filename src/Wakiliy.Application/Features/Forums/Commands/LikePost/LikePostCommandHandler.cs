using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;
using Wakiliy.Domain.Errors;

namespace Wakiliy.Application.Features.Forums.Commands.LikePost;

public class LikePostCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<LikePostCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LikePostCommand request, CancellationToken cancellationToken)
    {
        var post = await unitOfWork.Forums.GetPostByIdAsync(request.PostId, cancellationToken);
        if (post == null)
            return Result.Failure<string>(new Error("PostNotFound", "The forum post was not found.", 404));

        post.LikesCount++;
        unitOfWork.Forums.UpdatePost(post);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Post liked successfully.");
    }
}

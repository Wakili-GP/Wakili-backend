using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Application.Features.Forums.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;
using Wakiliy.Domain.Errors;

namespace Wakiliy.Application.Features.Forums.Commands.CreateComment;

public class CreateCommentCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCommentCommand, Result<ForumCommentResponse>>
{
    public async Task<Result<ForumCommentResponse>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.GetUserByIdAsync(request.AuthorId);
        if (user == null)
            return Result.Failure<ForumCommentResponse>(new Error("UserNotFound", "The user does not exist.", 404));

        var post = await unitOfWork.Forums.GetPostByIdAsync(request.PostId, cancellationToken);
        if (post == null)
            return Result.Failure<ForumCommentResponse>(new Error("PostNotFound", "The post does not exist.", 404));

        if (!string.IsNullOrEmpty(request.ParentId))
        {
            var parent = await unitOfWork.Forums.GetCommentByIdAsync(request.ParentId, cancellationToken);
            if (parent == null)
                return Result.Failure<ForumCommentResponse>(new Error("CommentNotFound", "The parent comment does not exist.", 404));
        }

        var comment = new ForumComment
        {
            Id = Guid.NewGuid().ToString(),
            Body = request.Body,
            PostId = request.PostId,
            AuthorId = request.AuthorId,
            ParentId = string.IsNullOrEmpty(request.ParentId) ? null : request.ParentId,
            CreatedAt = DateTime.UtcNow,
            LikesCount = 0
        };

        await unitOfWork.Forums.AddCommentAsync(comment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new ForumCommentResponse
        {
            Id = comment.Id,
            Body = comment.Body,
            CreatedAt = comment.CreatedAt,
            LikesCount = comment.LikesCount,
            IsLiked = false,
            ParentId = comment.ParentId,
            Author = new ForumAuthorResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfileImageUrl = null
            }
        };

        return Result.Success(response);
    }
}

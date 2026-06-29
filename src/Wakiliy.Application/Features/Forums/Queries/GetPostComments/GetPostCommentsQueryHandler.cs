using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Application.Features.Forums.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Forums.Queries.GetPostComments;

public class GetPostCommentsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetPostCommentsQuery, Result<List<ForumCommentResponse>>>
{
    public async Task<Result<List<ForumCommentResponse>>> Handle(GetPostCommentsQuery request, CancellationToken cancellationToken)
    {
        var comments = await unitOfWork.Forums.GetCommentsByPostIdAsync(request.PostId, cancellationToken);
        var response = comments.Select(MapToResponse).ToList();
        return Result.Success(response);
    }

    private ForumCommentResponse MapToResponse(ForumComment comment)
    {
        return new ForumCommentResponse
        {
            Id = comment.Id,
            Body = comment.Body,
            CreatedAt = comment.CreatedAt,
            LikesCount = comment.LikesCount,
            IsLiked = false,
            ParentId = comment.ParentId,
            Author = new ForumAuthorResponse
            {
                Id = comment.Author.Id,
                FirstName = comment.Author.FirstName,
                LastName = comment.Author.LastName,
                ProfileImageUrl = comment.Author.ProfileImage?.SystemFileUrl
            },
            Replies = comment.Replies?.Select(MapToResponse).ToList() ?? new List<ForumCommentResponse>()
        };
    }
}

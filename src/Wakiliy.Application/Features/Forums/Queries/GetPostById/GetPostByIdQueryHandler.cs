using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Application.Features.Forums.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;
using Wakiliy.Domain.Errors;

namespace Wakiliy.Application.Features.Forums.Queries.GetPostById;

public class GetPostByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetPostByIdQuery, Result<ForumPostResponse>>
{
    public async Task<Result<ForumPostResponse>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await unitOfWork.Forums.GetPostByIdAsync(request.PostId, cancellationToken);
        if (post == null)
            return Result.Failure<ForumPostResponse>(new Error("PostNotFound", "The forum post was not found.", 404));

        var response = new ForumPostResponse
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            CreatedAt = post.CreatedAt,
            Status = post.Status.ToString(),
            LikesCount = post.LikesCount,
            CommentsCount = post.Comments.Count,
            Tags = post.Tags.Select(t => t.Name).ToList(),
            IsLiked = false,
            Author = new ForumAuthorResponse
            {
                Id = post.Author.Id,
                FirstName = post.Author.FirstName,
                LastName = post.Author.LastName,
                ProfileImageUrl = post.Author.ProfileImage?.SystemFileUrl
            },
            Specialization = new ForumSpecializationResponse
            {
                Id = post.Specialization.Id,
                Name = post.Specialization.Name
            }
        };

        return Result.Success(response);
    }
}

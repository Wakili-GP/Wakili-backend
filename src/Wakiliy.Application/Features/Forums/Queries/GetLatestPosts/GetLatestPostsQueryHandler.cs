using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Application.Features.Forums.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Forums.Queries.GetLatestPosts;

public class GetLatestPostsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetLatestPostsQuery, Result<List<ForumPostResponse>>>
{
    public async Task<Result<List<ForumPostResponse>>> Handle(GetLatestPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await unitOfWork.Forums.GetLatestPostsAsync(request.Limit, cancellationToken);

        var items = posts.Select(p => new ForumPostResponse
        {
            Id = p.Id,
            Title = p.Title,
            Body = p.Body,
            CreatedAt = p.CreatedAt,
            Status = p.Status.ToString(),
            LikesCount = p.LikesCount,
            CommentsCount = p.Comments?.Count ?? 0,
            Tags = p.Tags?.Select(t => t.Name).ToList() ?? new List<string>(),
            IsLiked = false,
            Author = new ForumAuthorResponse
            {
                Id = p.Author.Id,
                FirstName = p.Author.FirstName,
                LastName = p.Author.LastName,
                ProfileImageUrl = p.Author.ProfileImage?.SystemFileUrl
            },
            Specialization = new ForumSpecializationResponse
            {
                Id = p.Specialization.Id,
                Name = p.Specialization.Name
            }
        }).ToList();

        return Result.Success(items);
    }
}

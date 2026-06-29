using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Application.Common.Models;
using Wakiliy.Application.Features.Forums.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Features.Forums.Queries.GetApprovedPosts;

public class GetApprovedPostsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetApprovedPostsQuery, Result<PaginatedResult<ForumPostResponse>>>
{
    public async Task<Result<PaginatedResult<ForumPostResponse>>> Handle(GetApprovedPostsQuery request, CancellationToken cancellationToken)
    {
        var (posts, totalCount) = await unitOfWork.Forums.GetPostsAsync(
            keyword: request.Keyword,
            specializationId: request.SpecializationId,
            sortBy: request.SortBy,
            page: request.Page,
            limit: request.Limit,
            onlyApproved: true,
            cancellationToken: cancellationToken
        );

        var items = posts.Select(p => new ForumPostResponse
        {
            Id = p.Id,
            Title = p.Title,
            Body = p.Body,
            CreatedAt = p.CreatedAt,
            Status = p.Status.ToString(),
            LikesCount = p.LikesCount,
            CommentsCount = p.Comments.Count,
            Tags = p.Tags.Select(t => t.Name).ToList(),
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

        var result = new PaginatedResult<ForumPostResponse>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.Limit,
            TotalCount = totalCount
        };

        return Result.Success(result);
    }
}

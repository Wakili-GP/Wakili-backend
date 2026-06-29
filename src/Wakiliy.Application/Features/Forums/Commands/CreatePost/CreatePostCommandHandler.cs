using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Application.Features.Forums.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;
using Wakiliy.Domain.Errors;

namespace Wakiliy.Application.Features.Forums.Commands.CreatePost;

public class CreatePostCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreatePostCommand, Result<ForumPostResponse>>
{
    public async Task<Result<ForumPostResponse>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.GetUserByIdAsync(request.AuthorId);
        if (user == null)
            return Result.Failure<ForumPostResponse>(new Error("UserNotFound", "The user does not exist.", 404));

        var specialization = await unitOfWork.Specializations.GetByIdAsync(request.SpecializationId, cancellationToken);
        if (specialization == null)
            return Result.Failure<ForumPostResponse>(SpecializationErrors.NotFound);

        var post = new ForumPost
        {
            Id = Guid.NewGuid().ToString(),
            Title = request.Title,
            Body = request.Body,
            SpecializationId = request.SpecializationId,
            AuthorId = request.AuthorId,
            Status = PostStatus.Pending,
            Tags = request.Tags.Select(t => new ForumPostTag { Name = t }).ToList(),
            CreatedAt = DateTime.UtcNow,
            LikesCount = 0
        };

        await unitOfWork.Forums.AddPostAsync(post, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new ForumPostResponse
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            CreatedAt = post.CreatedAt,
            Status = post.Status.ToString(),
            LikesCount = post.LikesCount,
            CommentsCount = 0,
            Tags = request.Tags,
            IsLiked = false,
            Author = new ForumAuthorResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfileImageUrl = null // UserReadModel doesn't expose this currently
            },
            Specialization = new ForumSpecializationResponse
            {
                Id = specialization.Id,
                Name = specialization.Name
            }
        };

        return Result.Success(response);
    }
}

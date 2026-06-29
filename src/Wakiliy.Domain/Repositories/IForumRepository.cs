using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Domain.Repositories;

public interface IForumRepository
{
    Task<(IEnumerable<ForumPost> Posts, int TotalCount)> GetPostsAsync(string? keyword, int? specializationId, string sortBy, int page, int limit, bool onlyApproved, CancellationToken cancellationToken = default);
    Task<IEnumerable<ForumPost>> GetLatestPostsAsync(int limit, CancellationToken cancellationToken = default);
    Task<ForumPost?> GetPostByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ForumComment>> GetCommentsByPostIdAsync(string postId, CancellationToken cancellationToken = default);
    Task<ForumComment?> GetCommentByIdAsync(string id, CancellationToken cancellationToken = default);
    
    Task AddPostAsync(ForumPost post, CancellationToken cancellationToken = default);
    Task AddCommentAsync(ForumComment comment, CancellationToken cancellationToken = default);
    void UpdatePost(ForumPost post);
    void UpdateComment(ForumComment comment);
    Task<int> GetTotalQuestionsAsync(CancellationToken cancellationToken = default);
    Task<int> GetTotalAnswersAsync(CancellationToken cancellationToken = default);
}

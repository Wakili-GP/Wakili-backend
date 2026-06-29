using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories;

public class ForumRepository(ApplicationDbContext context) : IForumRepository
{
    public async Task<(IEnumerable<ForumPost> Posts, int TotalCount)> GetPostsAsync(string? keyword, int? specializationId, string sortBy, int page, int limit, bool onlyApproved, CancellationToken cancellationToken = default)
    {
        var query = context.ForumPosts
            .Include(p => p.Author)
            .Include(p => p.Specialization)
            .Include(p => p.Tags)
            .AsQueryable();

        if (onlyApproved)
        {
            query = query.Where(p => p.Status == PostStatus.Approved);
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(p => p.Title.Contains(keyword) || p.Body.Contains(keyword));
        }

        if (specializationId.HasValue && specializationId.Value > 0)
        {
            query = query.Where(p => p.SpecializationId == specializationId.Value);
        }

        query = sortBy switch
        {
            "most_liked" => query.OrderByDescending(p => p.LikesCount),
            "most_commented" => query.OrderByDescending(p => p.Comments.Count),
            "unanswered" => query.Where(p => !p.Comments.Any()).OrderByDescending(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt), // default is 'newest'
        };

        var totalCount = await query.CountAsync(cancellationToken);
        
        var posts = await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return (posts, totalCount);
    }

    public async Task<IEnumerable<ForumPost>> GetLatestPostsAsync(int limit, CancellationToken cancellationToken = default)
    {
        return await context.ForumPosts
            .Include(p => p.Author)
            .Include(p => p.Specialization)
            .Where(p => p.Status == PostStatus.Approved)
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<ForumPost?> GetPostByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await context.ForumPosts
            .Include(p => p.Author)
            .Include(p => p.Specialization)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ForumComment>> GetCommentsByPostIdAsync(string postId, CancellationToken cancellationToken = default)
    {
        return await context.ForumComments
            .Include(c => c.Author)
            .Include(c => c.Replies)
            .Where(c => c.PostId == postId && c.ParentId == null) // Get root comments only
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ForumComment?> GetCommentByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await context.ForumComments.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task AddPostAsync(ForumPost post, CancellationToken cancellationToken = default)
    {
        await context.ForumPosts.AddAsync(post, cancellationToken);
    }

    public async Task AddCommentAsync(ForumComment comment, CancellationToken cancellationToken = default)
    {
        await context.ForumComments.AddAsync(comment, cancellationToken);
    }

    public void UpdatePost(ForumPost post)
    {
        context.ForumPosts.Update(post);
    }

    public void UpdateComment(ForumComment comment)
    {
        context.ForumComments.Update(comment);
    }
    
    public async Task<int> GetTotalQuestionsAsync(CancellationToken cancellationToken = default)
    {
        return await context.ForumPosts.CountAsync(p => p.Status == PostStatus.Approved, cancellationToken);
    }

    public async Task<int> GetTotalAnswersAsync(CancellationToken cancellationToken = default)
    {
        return await context.ForumComments.CountAsync(cancellationToken);
    }
}

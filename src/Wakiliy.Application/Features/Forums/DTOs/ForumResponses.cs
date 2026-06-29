namespace Wakiliy.Application.Features.Forums.DTOs;

public class ForumPostResponse
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int LikesCount { get; set; }
    public bool IsLiked { get; set; } // Computed dynamically
    public int CommentsCount { get; set; }

    public ForumAuthorResponse Author { get; set; } = null!;
    public ForumSpecializationResponse Specialization { get; set; } = null!;
    public List<string> Tags { get; set; } = new();
}

public class ForumAuthorResponse
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
}

public class ForumSpecializationResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ForumCommentResponse
{
    public string Id { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int LikesCount { get; set; }
    public bool IsLiked { get; set; }
    public string? ParentId { get; set; }

    public ForumAuthorResponse Author { get; set; } = null!;
    public List<ForumCommentResponse> Replies { get; set; } = new();
}

public class ForumStatsResponse
{
    public int TotalQuestions { get; set; }
    public int TotalAnswers { get; set; }
    public int ActiveUsers { get; set; }
    public int ResolvedQuestions { get; set; }
}

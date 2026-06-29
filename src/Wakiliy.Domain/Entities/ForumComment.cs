using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wakiliy.Domain.Entities;

public class ForumComment
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string Body { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int LikesCount { get; set; } = 0;

    // Foreign Keys
    [Required]
    public string PostId { get; set; } = string.Empty;
    [ForeignKey(nameof(PostId))]
    public ForumPost Post { get; set; } = null!;

    [Required]
    public string AuthorId { get; set; } = string.Empty;
    [ForeignKey(nameof(AuthorId))]
    public AppUser Author { get; set; } = null!;

    public string? ParentId { get; set; }
    [ForeignKey(nameof(ParentId))]
    public ForumComment? ParentComment { get; set; }

    // Navigation Properties
    public ICollection<ForumComment> Replies { get; set; } = new List<ForumComment>();
}

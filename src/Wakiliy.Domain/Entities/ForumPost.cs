using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wakiliy.Domain.Entities;

public class ForumPost
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public PostStatus Status { get; set; } = PostStatus.Pending;

    public int LikesCount { get; set; } = 0;

    // Foreign Keys
    [Required]
    public string AuthorId { get; set; } = string.Empty;
    [ForeignKey(nameof(AuthorId))]
    public AppUser Author { get; set; } = null!;

    [Required]
    public int SpecializationId { get; set; }
    [ForeignKey(nameof(SpecializationId))]
    public Specialization Specialization { get; set; } = null!;

    // Navigation Properties
    public ICollection<ForumComment> Comments { get; set; } = new List<ForumComment>();
    public ICollection<ForumPostTag> Tags { get; set; } = new List<ForumPostTag>();
}

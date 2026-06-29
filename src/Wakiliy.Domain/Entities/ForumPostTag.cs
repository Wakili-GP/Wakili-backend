using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wakiliy.Domain.Entities;

public class ForumPostTag
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string PostId { get; set; } = string.Empty;
    [ForeignKey(nameof(PostId))]
    public ForumPost Post { get; set; } = null!;
}

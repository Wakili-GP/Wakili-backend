using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Infrastructure.Data.Config;

public class NotificationConnectionSessionConfiguration : IEntityTypeConfiguration<NotificationConnectionSession>
{
    public void Configure(EntityTypeBuilder<NotificationConnectionSession> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(s => s.ConnectionId)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(s => s.ConnectionId).IsUnique();
        builder.HasIndex(s => s.UserId);

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

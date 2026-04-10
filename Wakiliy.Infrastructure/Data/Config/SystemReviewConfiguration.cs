using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Infrastructure.Data.Config;

internal class SystemReviewConfiguration : IEntityTypeConfiguration<SystemReview>
{
    public void Configure(EntityTypeBuilder<SystemReview> builder)
    {
        builder.HasKey(sr => sr.Id);

        builder.Property(sr => sr.Rating)
            .IsRequired();

        builder.Property(sr => sr.Comment)
            .HasMaxLength(500);

        // Configure AiAnalysis as an owned entity (embedded in the same table)
        builder.OwnsOne(sr => sr.AiAnalysis, ai =>
        {
            ai.Property(a => a.IsFlagged).HasColumnName("AiAnalysis_IsFlagged");
            ai.Property(a => a.Confidence).HasColumnName("AiAnalysis_Confidence");
            ai.Property(a => a.Summary).HasColumnName("AiAnalysis_Summary").HasMaxLength(1000);
        });

        builder.HasOne(sr => sr.User)
            .WithMany()
            .HasForeignKey(sr => sr.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

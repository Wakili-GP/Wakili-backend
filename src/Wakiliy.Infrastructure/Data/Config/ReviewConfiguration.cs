using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Infrastructure.Data.Config;

internal class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(500);

        builder.HasIndex(r => r.AppointmentId)
            .IsUnique();

        // Configure AiAnalysis as an owned entity 
        builder.OwnsOne(r => r.AiAnalysis, ai =>
        {
            ai.Property(a => a.IsFlagged).HasColumnName("AiAnalysis_IsFlagged");
            ai.Property(a => a.Confidence).HasColumnName("AiAnalysis_Confidence");
            ai.Property(a => a.Summary).HasColumnName("AiAnalysis_Summary").HasMaxLength(1000);
        });

        builder.HasOne(r => r.Appointment)
            .WithMany()
            .HasForeignKey(r => r.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Lawyer)
            .WithMany(l=>l.Reviews)
            .HasForeignKey(r => r.LawyerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

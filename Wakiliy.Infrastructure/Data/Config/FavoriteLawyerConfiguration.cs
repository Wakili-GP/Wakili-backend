using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Infrastructure.Data.Config;

internal class FavoriteLawyerConfiguration : IEntityTypeConfiguration<FavoriteLawyer>
{
    public void Configure(EntityTypeBuilder<FavoriteLawyer> builder)
    {
        builder.HasKey(fl => new { fl.UserId, fl.LawyerId });

        builder.HasOne(fl => fl.User)
            .WithMany(u => u.FavoriteLawyers)
            .HasForeignKey(fl => fl.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(fl => fl.Lawyer)
            .WithMany()
            .HasForeignKey(fl => fl.LawyerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

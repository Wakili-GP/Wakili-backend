using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Infrastructure.Data.Config;
internal class UserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {

        builder.Property(b => b.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.Gender)
            .HasMaxLength(20);


        builder.Property(b => b.Address)
            .HasMaxLength(250);



        builder.Property(b => b.ImageUrl)
            .HasMaxLength(500);


        builder.UseTptMappingStrategy();
    }
}
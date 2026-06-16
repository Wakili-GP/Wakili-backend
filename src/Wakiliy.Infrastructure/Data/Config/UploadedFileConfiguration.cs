using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Infrastructure.Data.Config
{
    internal class UploadedFileConfiguration : IEntityTypeConfiguration<UploadedFile>
    {
        public void Configure(EntityTypeBuilder<UploadedFile> builder)
        {
            builder.ToTable("UploadedFiles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FileName).IsRequired().HasMaxLength(255);
            builder.Property(x => x.SystemFileUrl).IsRequired();
            builder.Property(x => x.PublicId).IsRequired();

            builder.Property(x => x.ContentType).HasMaxLength(100);

            builder.Property(x => x.Category)
                .HasConversion<string>();

            builder.Property(x => x.Purpose)
                .HasConversion<string>();

        }
    }
}
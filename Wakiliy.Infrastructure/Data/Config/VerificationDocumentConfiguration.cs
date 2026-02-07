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
    internal class VerificationDocumentConfiguration
    : IEntityTypeConfiguration<VerificationDocuments>
    {
        public void Configure(EntityTypeBuilder<VerificationDocuments> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                .HasConversion<string>()
                .IsRequired();

            builder.HasOne(x => x.Lawyer)
                .WithMany(l => l.VerificationDocuments)
                .HasForeignKey(x => x.LawyerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.File)
                .WithMany()
                .HasForeignKey(x => x.FileId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

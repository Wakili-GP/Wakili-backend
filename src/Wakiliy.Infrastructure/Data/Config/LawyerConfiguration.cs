using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Infrastructure.Data.Config;

internal class LawyerConfiguration : IEntityTypeConfiguration<Lawyer>
{

    public void Configure(EntityTypeBuilder<Lawyer> builder)
    {
        builder.Property(l => l.Country)
            .HasMaxLength(100);

        builder.Property(l => l.City)
            .HasMaxLength(100);

        builder.Property(l => l.Bio)
            .HasMaxLength(1000);

        builder.HasMany(l => l.Specializations)
            .WithMany(s => s.Lawyers)
            .UsingEntity<Dictionary<string, object>>(
                "LawyerSpecializations",
                j => j.HasOne<Specialization>()
                      .WithMany()
                      .HasForeignKey("SpecializationId")
                      .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Lawyer>()
                      .WithMany()
                      .HasForeignKey("LawyerId")
                      .OnDelete(DeleteBehavior.Cascade))
            .ToTable("LawyerSpecializations");

        
       
        builder.HasMany(l => l.AcademicQualifications)
            .WithOne(q => q.Lawyer)
            .HasForeignKey(q => q.LawyerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(l => l.ProfessionalCertifications)
            .WithOne(c => c.Lawyer)
            .HasForeignKey(c => c.LawyerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(l => l.WorkExperiences)
            .WithOne(e => e.Lawyer)
            .HasForeignKey(e => e.LawyerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(l => l.VerificationDocuments)
            .WithOne(v=>v.Lawyer)
            .HasForeignKey(v => v.LawyerId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}

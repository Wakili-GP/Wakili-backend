using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Infrastructure.Data.Config;

internal class VerificationDocumentsConfiguration : IEntityTypeConfiguration<VerificationDocuments>
{
    public void Configure(EntityTypeBuilder<VerificationDocuments> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.LawyerId)
            .IsRequired();

        //builder.OwnsOne(v => v.NationalIdFront, nav =>
        //{
        //    nav.Property(d => d.FilePath)
        //        .HasColumnName("NationalIdFrontPath")
        //        .HasMaxLength(500);

        //    nav.Property(d => d.FileName)
        //        .HasColumnName("NationalIdFrontFileName")
        //        .HasMaxLength(250);

        //    nav.Property(d => d.Status)
        //        .HasColumnName("NationalIdFrontStatus");
        //});

        //builder.OwnsOne(v => v.NationalIdBack, nav =>
        //{
        //    nav.Property(d => d.FilePath)
        //        .HasColumnName("NationalIdBackPath")
        //        .HasMaxLength(500);

        //    nav.Property(d => d.FileName)
        //        .HasColumnName("NationalIdBackFileName")
        //        .HasMaxLength(250);

        //    nav.Property(d => d.Status)
        //        .HasColumnName("NationalIdBackStatus");
        //});

        //builder.OwnsOne(v => v.LawyerLicense, nav =>
        //{
        //    nav.Property(d => d.FilePath)
        //        .HasColumnName("LawyerLicensePath")
        //        .HasMaxLength(500);

        //    nav.Property(d => d.FileName)
        //        .HasColumnName("LawyerLicenseFileName")
        //        .HasMaxLength(250);

        //    nav.Property(d => d.Status)
        //        .HasColumnName("LawyerLicenseStatus");
        //});

        //builder.OwnsMany(v => v.EducationalCertificates, nav =>
        //{
        //    nav.ToTable("VerificationEducationalCertificates");
        //    nav.WithOwner().HasForeignKey("VerificationDocumentsId");
        //    nav.Property<Guid>("Id");
        //    nav.HasKey("Id");
        //    nav.Property(d => d.FilePath)
        //        .HasMaxLength(500);
        //    nav.Property(d => d.FileName)
        //        .HasMaxLength(250);
        //    nav.Property(d => d.Status);
        //});

        //builder.OwnsMany(v => v.ProfessionalCertificates, nav =>
        //{
        //    nav.ToTable("VerificationProfessionalCertificates");
        //    nav.WithOwner().HasForeignKey("VerificationDocumentsId");
        //    nav.Property<Guid>("Id");
        //    nav.HasKey("Id");
        //    nav.Property(d => d.FilePath)
        //        .HasMaxLength(500);
        //    nav.Property(d => d.FileName)
        //        .HasMaxLength(250);
        //    nav.Property(d => d.Status);
        //});
    }
}

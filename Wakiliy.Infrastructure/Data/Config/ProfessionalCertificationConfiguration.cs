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
    internal class ProfessionalCertificationConfiguration
    : IEntityTypeConfiguration<ProfessionalCertification>
    {
        public void Configure(EntityTypeBuilder<ProfessionalCertification> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Document)
                .WithMany()
                .HasForeignKey(x => x.DocumentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}

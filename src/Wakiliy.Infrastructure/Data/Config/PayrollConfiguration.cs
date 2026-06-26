using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Infrastructure.Data.Config;

internal class PayrollConfiguration : IEntityTypeConfiguration<Payroll>
{
    public void Configure(EntityTypeBuilder<Payroll> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.TotalAmount).HasPrecision(18, 2);

        builder.HasOne(p => p.Lawyer)
            .WithMany(l => l.Payrolls)
            .HasForeignKey(p => p.LawyerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

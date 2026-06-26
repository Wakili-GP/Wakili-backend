using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Infrastructure.Data.Config;

internal class LawyerEarningConfiguration : IEntityTypeConfiguration<LawyerEarning>
{
    public void Configure(EntityTypeBuilder<LawyerEarning> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.GrossAmount).HasPrecision(18, 2);
        builder.Property(e => e.PlatformFee).HasPrecision(18, 2);
        builder.Property(e => e.NetAmount).HasPrecision(18, 2);

        builder.HasOne(e => e.Lawyer)
            .WithMany(l => l.Earnings)
            .HasForeignKey(e => e.LawyerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Payroll)
            .WithMany(p => p.Earnings)
            .HasForeignKey(e => e.PayrollId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

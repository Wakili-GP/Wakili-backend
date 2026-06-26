using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wakiliy.Domain.Entities;

namespace Wakiliy.Infrastructure.Data.Config;

internal class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.Slot)
            .WithMany()
            .HasForeignKey(a => a.SlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Client)
            .WithMany()
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Lawyer)
            .WithMany(l => l.Appointments)
            .HasForeignKey(a => a.LawyerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.PaymentTransaction)
            .WithOne(p => p.Appointment)
            .HasForeignKey<Appointment>(a => a.PaymentTransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.LawyerEarning)
            .WithOne(e => e.Appointment)
            .HasForeignKey<LawyerEarning>(e => e.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

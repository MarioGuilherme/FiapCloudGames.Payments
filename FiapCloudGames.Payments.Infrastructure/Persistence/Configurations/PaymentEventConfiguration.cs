using FiapCloudGames.Payments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiapCloudGames.Payments.Infrastructure.Persistence.Configurations;

public class PaymentEventConfiguration : IEntityTypeConfiguration<PaymentEvent>
{
    public void Configure(EntityTypeBuilder<PaymentEvent> builder)
    {
        builder.HasKey(pe => pe.PaymentEventId);

        builder.Property(pe => pe.UserId).IsRequired();

        builder.Property(pe => pe.PaymentId).IsRequired();

        builder.Property(pe => pe.OrderId).IsRequired();

        builder.Property(pe => pe.ExternalId).IsRequired(false);

        builder.Property(pe => pe.Total).HasPrecision(18, 2);

        builder.HasOne(pe => pe.Payment)
               .WithMany(p => p.PaymentEvents)
               .HasForeignKey(pe => pe.PaymentId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("PaymentEvents");
    }
}

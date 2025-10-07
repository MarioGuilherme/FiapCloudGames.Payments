using FiapCloudGames.Payments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiapCloudGames.Payments.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.PaymentId);

        builder.Property(p => p.UserId).IsRequired();

        builder.Property(p => p.PaymentId).IsRequired();

        builder.Property(p => p.OrderId).IsRequired();

        builder.Property(p => p.ExternalId).IsRequired(false);

        builder.Property(p => p.Total).HasPrecision(18, 2);

        builder.HasMany(p => p.PaymentEvents)
               .WithOne(pe => pe.Payment)
               .HasForeignKey(pe => pe.PaymentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

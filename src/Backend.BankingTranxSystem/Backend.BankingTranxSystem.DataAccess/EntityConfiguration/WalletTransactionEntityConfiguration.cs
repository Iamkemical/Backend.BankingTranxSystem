using Backend.BankingTranxSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.BankingTranxSystem.DataAccess.EntityConfiguration;

public class WalletTransactionEntityConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DestinationReference)
               .HasMaxLength(10)
               .IsRequired();

        builder.Property(x => x.SourceReference)
               .HasMaxLength(10)
               .IsRequired();

        builder.Property(x => x.Amount)
               .HasColumnType("decimal(18,4)");

        builder.Property(x => x.Narration)
               .HasMaxLength(500);
    }
}
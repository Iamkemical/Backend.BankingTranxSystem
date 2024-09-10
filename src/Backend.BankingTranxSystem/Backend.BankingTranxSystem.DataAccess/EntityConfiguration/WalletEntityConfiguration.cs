using Backend.BankingTranxSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.BankingTranxSystem.DataAccess.EntityConfiguration;

public class WalletEntityConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Reference)
               .HasMaxLength(10)
               .IsRequired();

        builder.HasOne(wallet => wallet.User)
               .WithOne(User => User.Wallet)
               .HasForeignKey<Wallet>(wallet => wallet.UserId);

        builder.Property(x => x.Balance)
               .HasColumnType("decimal(18,4)")
               .IsRequired();
    }
}
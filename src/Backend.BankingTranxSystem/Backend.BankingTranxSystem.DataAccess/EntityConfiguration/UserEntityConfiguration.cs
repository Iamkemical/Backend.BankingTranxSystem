using Backend.BankingTranxSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.BankingTranxSystem.DataAccess.EntityConfiguration;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.LastName)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.OtherNames)
               .HasMaxLength(200);

        builder.Property(x => x.Password)
               .HasMaxLength(500);

        builder.Property(x => x.State)
               .HasMaxLength(50);

        builder.Property(x => x.Country)
               .HasMaxLength(150);

        builder.Property(x => x.PermanentAddress)
               .HasMaxLength(350);

        builder.Property(x => x.Bvn)
               .HasMaxLength(10);

        builder.Property(x => x.BusinessRegistrationNumber)
               .HasMaxLength(50);

        builder.Property(x => x.EmailAddress)
               .HasMaxLength(300);

        builder.Property(x => x.TelephoneNumber)
               .HasMaxLength(50);
    }
}
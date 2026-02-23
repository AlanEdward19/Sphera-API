using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Billing.BilletConfigurations;
using Sphera.API.Billing.Billets;

namespace Sphera.API.External.Database.Maps;

public class BilletConfigurationMap : IEntityTypeConfiguration<BilletConfiguration>
{
    public void Configure(EntityTypeBuilder<BilletConfiguration> b)
    {
        b.ToTable("BilletConfigurations");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWID()");

        b.Property(x => x.CompanyCode).HasMaxLength(20);
        b.Property(x => x.CompanyName).HasMaxLength(30);
        b.Property(x => x.WalletNumber).HasMaxLength(3);
        b.Property(x => x.AgencyNumber).HasMaxLength(5);
        b.Property(x => x.AccountNumber).HasMaxLength(7);
        b.Property(x => x.AccountDigit).HasMaxLength(1);
        b.Property(x => x.BankCode).HasMaxLength(3);
        b.Property(x => x.HasFine);
        b.Property(x => x.FinePercentage).HasColumnType("decimal(18,2)");
        b.Property(x => x.DailyDiscount).HasColumnType("decimal(18,2)");
        b.Property(x => x.DailyInterest).HasColumnType("decimal(18,2)");
        b.Property(x => x.DiscountLimitDate).HasColumnType("date");
        b.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
        b.Property(x => x.RebateAmount).HasColumnType("decimal(18,2)");
        b.Property(x => x.FirstMessage).HasMaxLength(12);
        b.Property(x => x.SecondMessage).HasMaxLength(60);

        b.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        b.Property(x => x.CreatedBy).HasColumnType("uniqueidentifier").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnType("datetime2");
        b.Property(x => x.UpdatedBy).HasColumnType("uniqueidentifier");
    }
}

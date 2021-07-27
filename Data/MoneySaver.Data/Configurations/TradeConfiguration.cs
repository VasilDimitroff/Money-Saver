namespace MoneySaver.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using MoneySaver.Data.Models;

    public class TradeConfiguration : IEntityTypeConfiguration<Trade>
    {
        public void Configure(EntityTypeBuilder<Trade> trade)
        {
            trade
                .HasKey(ut => new { ut.Id });

            trade
                .HasOne(tr => tr.Company)
               .WithMany(company => company.Trades)
               .HasForeignKey(tr => tr.CompanyId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            trade
              .HasOne(tr => tr.InvestmentWallet)
              .WithMany(iw => iw.Trades)
              .HasForeignKey(tr => tr.InvestmentWalletId)
              .IsRequired()
              .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

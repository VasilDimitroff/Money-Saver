namespace MoneySaver.Data.Configurations
{
    using MoneySaver.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class WalletsCategoriesConfiguration : IEntityTypeConfiguration<WalletCategory>
    {
        public void Configure(EntityTypeBuilder<WalletCategory> walletCategory)
        {
            walletCategory
                .HasKey(wc => new { wc.CategoryId, wc.WalletId });

            walletCategory
                .HasOne(wallet => wallet.Wallet)
                .WithMany(w => w.Categories)
                .HasForeignKey(w => w.WalletId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            walletCategory
                .HasOne(wc => wc.Category)
               .WithMany(c => c.Wallets)
               .HasForeignKey(c => c.CategoryId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

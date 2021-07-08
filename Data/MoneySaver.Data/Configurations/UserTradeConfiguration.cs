namespace MoneySaver.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using MoneySaver.Data.Models;

    public class UserTradeConfiguration : IEntityTypeConfiguration<UserTrade>
    {
        public void Configure(EntityTypeBuilder<UserTrade> userTrade)
        {
            userTrade
                .HasKey(ut => new { ut.Id });

            userTrade
                .HasOne(trade => trade.ApplicationUser)
                .WithMany(user => user.Trades)
                .HasForeignKey(trade => trade.ApplicationUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            userTrade
                .HasOne(trade => trade.Company)
               .WithMany(company => company.Traders)
               .HasForeignKey(trade => trade.CompanyTicker)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

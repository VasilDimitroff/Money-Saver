namespace MoneySaver.Data.Configurations
{
    using MoneySaver.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
                .HasOne(trade => trade.Stock)
               .WithMany(stock => stock.Trades)
               .HasForeignKey(trade => trade.StockId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

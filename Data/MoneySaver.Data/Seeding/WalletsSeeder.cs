namespace MoneySaver.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using MoneySaver.Data.Models;

    public class WalletsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Wallets.Any())
            {
                return;
            }

            await dbContext.Wallets
                .AddAsync(new Wallet { Name = "My Home Wallet", ApplicationUserId = "first", MoneyAmount = 1000, CurrencyId = 12 });
        }
    }
}

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
                .AddAsync(new Wallet { Name = "Home Wallet", ApplicationUserId = "70cb89bc-f984-4977-9fdd-ee6c7efa63c6", MoneyAmount = 1000, CurrencyId = 1 });
        }
    }
}

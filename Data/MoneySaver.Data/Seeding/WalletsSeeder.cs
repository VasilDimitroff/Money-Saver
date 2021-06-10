namespace MoneySaver.Data.Seeding
{
    using MoneySaver.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class WalletsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Wallets.Any())
            {
                return;
            }

            await dbContext.Wallets
                .AddAsync(new Wallet { Name = "Default Wallet", ApplicationUserId = "first", MoneyAmount = 1000, CurrencyId = 1 });
        }
    }
}

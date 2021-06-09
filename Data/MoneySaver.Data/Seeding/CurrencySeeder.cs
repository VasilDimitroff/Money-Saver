namespace MoneySaver.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using MoneySaver.Data.Models;

    internal class CurrencySeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Currencies.Any())
            {
                return;
            }

            await dbContext.Currencies.AddAsync(new Currency { Name = "Bulgarian Lev", Code = "BGN" });
            await dbContext.Currencies.AddAsync(new Currency { Name = "Euro", Code = "EUR" });
            await dbContext.Currencies.AddAsync(new Currency { Name = "US Dollar", Code = "USD" });
            await dbContext.Currencies.AddAsync(new Currency { Name = "British Pound", Code = "GBP" });
        }
    }
}

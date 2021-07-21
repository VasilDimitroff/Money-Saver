namespace MoneySaver.Data.Seeding
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using MoneySaver.Data.Models;

    public class CompaniesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Companies.Any())
            {
                return;
            }

            await dbContext.Companies
                .AddRangeAsync(
                new Company
                {
                    Name = "Facebook",
                    Ticker = "FB",
                },
                new Company
                {
                    Name = "Amazon",
                    Ticker = "AMZN",
                },
                new Company
                {
                    Name = "Apple",
                    Ticker = "AAPL",
                },
                new Company
                {
                    Name = "Tesla",
                    Ticker = "TSLA",
                },
                new Company
                {
                    Name = "Netflix",
                    Ticker = "NFLX",
                },
                new Company
                 {
                     Name = "Moderna",
                     Ticker = "MRNA",
                 });
        }
    }
}

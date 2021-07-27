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
                    Id = Guid.NewGuid().ToString(),
                    Name = "Facebook",
                    Ticker = "FB",
                },
                new Company
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Amazon",
                    Ticker = "AMZN",
                },
                new Company
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Apple",
                    Ticker = "AAPL",
                },
                new Company
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Tesla",
                    Ticker = "TSLA",
                },
                new Company
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Netflix",
                    Ticker = "NFLX",
                },
                new Company
                 {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Moderna",
                    Ticker = "MRNA",
                 });
        }
    }
}

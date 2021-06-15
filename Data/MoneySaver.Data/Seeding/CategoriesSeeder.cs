namespace MoneySaver.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using MoneySaver.Data.Models;

    public class CategoriesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Categories.Any())
            {
                return;
            }

            await dbContext.Categories.AddAsync(new Category { Name = "Grocery", WalletId = 5 });
            await dbContext.Categories.AddAsync(new Category { Name = "Alcohol", WalletId = 5 });
            await dbContext.Categories.AddAsync(new Category { Name = "Home", WalletId = 5 });
            await dbContext.Categories.AddAsync(new Category { Name = "Beauty", WalletId = 5 });
            await dbContext.Categories.AddAsync(new Category { Name = "Salary", WalletId = 5 });
         }
    }
}

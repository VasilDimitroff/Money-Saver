namespace MoneySaver.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using MoneySaver.Common;
    using MoneySaver.Data;
    using MoneySaver.Data.Models;

    public class CategoriesService : ICategoriesService
    {
        private readonly ApplicationDbContext dbContext;

        public CategoriesService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> AddAsync(string userId, string name)
        {
            Category category = await this.dbContext.Categories
                .FirstOrDefaultAsync(categ => categ.Name == name && categ.Records.Any(r => r.Wallet.ApplicationUserId == userId));

            if (category != null)
            {
                throw new ArgumentException(GlobalConstants.ExistingCategory);
            }

            category.Name = name;

            await this.dbContext.Categories.AddAsync(category);
            await this.dbContext.SaveChangesAsync();

            string successMessage = string.Format(GlobalConstants.SuccessfullyAddedCategory, category.Name);

            return successMessage;
        }

        public async Task<string> RemoveAsync(string userId, string name)
        {
            Category category = await this.dbContext.Categories
                .FirstOrDefaultAsync(categ => categ.Name == name && categ.Records.Any(r => r.Wallet.ApplicationUserId == userId));

            if (category == null)
            {
                throw new ArgumentException(string.Format(GlobalConstants.UnexistingCategory, name));
            }

            this.dbContext.Categories.Remove(category);
            await this.dbContext.SaveChangesAsync();

            string successMessage = string.Format(GlobalConstants.SuccessfullyRemovedCategory, category.Name);

            return successMessage;
        }
    }
}

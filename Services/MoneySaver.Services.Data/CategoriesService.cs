namespace MoneySaver.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using MoneySaver.Common;
    using MoneySaver.Data;
    using MoneySaver.Data.Models;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Services.Data.Models;

    public class CategoriesService : ICategoriesService
    {
        private readonly ApplicationDbContext dbContext;

        public CategoriesService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> AddAsync(string categoryName, int walletId)
        {
            Wallet targetWallet = await this.GetWalletByIdAsync(walletId);

            if (targetWallet == null)
            {
                throw new ArgumentNullException(GlobalConstants.NullValueOfWallet);
            }

            Category category = await this.dbContext.Categories
                .FirstOrDefaultAsync(categ => categ.Name.ToLower() == categoryName.ToLower()
                    && categ.Records.Any(record => record.WalletId == targetWallet.Id));

            if (category != null)
            {
                throw new ArgumentException(GlobalConstants.ExistingCategory);
            }

            category.Name = categoryName;

            await this.dbContext.Categories.AddAsync(category);
            await this.dbContext.SaveChangesAsync();

            string successMessage = string.Format(GlobalConstants.SuccessfullyAddedCategory, category.Name);

            return successMessage;
        }

        public async Task<string> RemoveAsync(int categoryId)
        {
            Category category = await this.dbContext.Categories
                .FirstOrDefaultAsync(categ => categ.Id == categoryId);

            if (category == null)
            {
                throw new ArgumentException(GlobalConstants.UnexistingCategory);
            }

            this.dbContext.Categories.Remove(category);
            await this.dbContext.SaveChangesAsync();

            string successMessage = string.Format(GlobalConstants.SuccessfullyRemovedCategory, category.Name);

            return successMessage;
        }

        public async Task<CategoryInfoDto> GetCategoryAsync(int categoryId)
        {
            var category = await this.dbContext.Categories
                .Where(c => c.Id == categoryId)
                .ToListAsync();

            var categoryDto = category
                .Select(category => new CategoryInfoDto
                {
                    Name = category.Name,
                })
                .FirstOrDefault();

            if (category == null)
            {
                throw new ArgumentException(GlobalConstants.UnexistingCategory);
            }

            return categoryDto;
        }

        private async Task<Wallet> GetWalletByIdAsync(int walletId)
        {
            Wallet targetWallet = await this.dbContext.Wallets
              .FirstOrDefaultAsync(w => w.Id == walletId);

            return targetWallet;
        }
    }
}

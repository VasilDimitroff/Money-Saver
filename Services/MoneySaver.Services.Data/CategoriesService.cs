﻿namespace MoneySaver.Services.Data
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
            Wallet wallet = await this.dbContext.Wallets.FirstOrDefaultAsync(x => x.Id == walletId);

            if (wallet == null)
            {
                throw new ArgumentException(GlobalConstants.WalletNotExist);
            }

            if (await this.IsCategoryExistAsync(walletId, categoryName))
            {
                throw new ArgumentException(GlobalConstants.ExistingCategory);
            }

            Category category = new Category
            {
                Name = categoryName,
                WalletId = wallet.Id,
            };

            await this.dbContext.Categories.AddAsync(category);
            await this.dbContext.SaveChangesAsync();

            return string.Format(GlobalConstants.SuccessfullyAddedCategory, category.Name);
        }

        public async Task<CategoryWalletInfoDto> GetCategoryAsync(int categoryId)
        {
            CategoryWalletInfoDto category = await this.dbContext.Categories
                .Where(x => x.Id == categoryId)
                .Select(categ => new CategoryWalletInfoDto
                {
                    CategoryName = categ.Name,
                    WalletName = categ.Wallet.Name,
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                throw new ArgumentException(GlobalConstants.UnexistingCategory);
            }

            return category;
        }

        public async Task<string> RemoveAsync(int categoryId)
        {
            Category category = await this.dbContext.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);

            if (category == null)
            {
                throw new ArgumentException(GlobalConstants.UnexistingCategory);
            }

            this.dbContext.Categories.Remove(category);
            await this.dbContext.SaveChangesAsync();

            return string.Format(GlobalConstants.SuccessfullyRemovedCategory, category.Name);
        }

        private async Task<bool> IsCategoryExistAsync(int walletId, string categoryName)
        {
            return await this.dbContext.Categories
                .AnyAsync(cat => cat.WalletId == walletId && cat.Name.ToLower() == categoryName.ToLower());
        }
    }
}

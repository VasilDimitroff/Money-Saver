namespace MoneySaver.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using MoneySaver.Common;
    using MoneySaver.Data;
    using MoneySaver.Data.Models;
    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Services.Data.Models;
    using MoneySaver.Services.Data.Models.Categories;
    using MoneySaver.Services.Data.Models.Records;
    using MoneySaver.Services.Data.Models.Wallets;

    public class CategoriesService : ICategoriesService
    {
        private readonly ApplicationDbContext dbContext;

        public CategoriesService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> AddAsync(string categoryName, int walletId, string badgeColor)
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

            BadgeColor badge;

            if (!Enum.TryParse<BadgeColor>(badgeColor, out badge))
            {
                throw new ArgumentException(GlobalConstants.BadgeColorNotValid);
            }

            Category category = new Category
            {
                Name = categoryName,
                WalletId = wallet.Id,
                CreatedOn = DateTime.UtcNow,
                BadgeColor = badge,
                ModifiedOn = DateTime.UtcNow,
            };

            var result = await this.dbContext.Categories.AddAsync(category);
            await this.dbContext.SaveChangesAsync();

            return string.Format(GlobalConstants.SuccessfullyAddedCategory, category.Name);
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

        public async Task<string> EditAsync(int categoryId, string categoryName, int walletId, string badgeColor)
        {
            var category = await this.dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                throw new ArgumentNullException(GlobalConstants.UnexistingCategory);
            }

            var wallet = await this.dbContext.Wallets.FirstOrDefaultAsync(w => w.Id == walletId);

            if (wallet == null)
            {
                throw new ArgumentNullException(GlobalConstants.WalletNotExist);
            }

            if (!Enum.TryParse<BadgeColor>(badgeColor, out BadgeColor badge))
            {
                throw new ArgumentException(GlobalConstants.BadgeColorNotValid);
            }

            category.BadgeColor = badge;
            category.ModifiedOn = DateTime.UtcNow;
            category.Name = categoryName;
            category.WalletId = walletId;

            await this.dbContext.SaveChangesAsync();

            return GlobalConstants.CategorySuccessfullyUpdated;
        }

        public async Task<AllRecordsInCategoryDto> GetRecordsByCategoryAsync(int categoryId)
        {
            var category = await this.dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                throw new ArgumentException(GlobalConstants.UnexistingCategory);
            }

            var categ = await this.dbContext.Categories
                 .Where(c => c.Id == categoryId)
                 .Select(r => new AllRecordsInCategoryDto
                 {
                     Category = r.Name,
                     CategoryId = r.Id,
                     Currency = r.Wallet.Currency.Code,
                     WalletId = r.WalletId,
                     WalletName = r.Wallet.Name,
                     BadgeColor = r.BadgeColor,
                     Records = r.Records.Select(r => new CategoryRecordInfoDto
                     {
                         Id = r.Id,
                         Amount = r.Amount,
                         CreatedOn = r.CreatedOn,
                         Description = r.Description,
                         Type = r.Type,
                     })
                     .OrderByDescending(rec => rec.CreatedOn)
                     .ThenBy(rec => rec.Amount)
                     .ToList(),
                 })
                 .FirstOrDefaultAsync();

            return categ;
        }

        public async Task<EditCategoryDto> GetCategoryInfoForEditAsync(int categoryId)
        {
            var category = await this.dbContext.Categories
                .Where(c => c.Id == categoryId)
                .Select(c => new EditCategoryDto
                {
                    BadgeColor = c.BadgeColor.ToString(),
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    WalletId = c.WalletId,
                    WalletName = c.Wallet.Name,
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                throw new ArgumentException(GlobalConstants.UnexistingCategory);
            }

            return category;
        }

        public async Task<IEnumerable<WalletNameAndIdDto>> GetAllWalletsWithNameAndIdAsync(string userId)
        {
            var user = await this.dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new ArgumentNullException(GlobalConstants.UserNotExist);
            }

            var wallets = await this.dbContext.Wallets
                .Where(w => w.ApplicationUserId == userId)
                .Select(w => new WalletNameAndIdDto
                {
                    WalletId = w.Id,
                    WalletName = w.Name,
                })
                .ToListAsync();

            return wallets;
        }

        private async Task<bool> IsCategoryExistAsync(int walletId, string categoryName)
        {
            return await this.dbContext.Categories
                .AnyAsync(cat => cat.WalletId == walletId && cat.Name.ToLower() == categoryName.ToLower());
        }
    }
}

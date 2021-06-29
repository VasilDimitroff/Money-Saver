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
        private readonly IRecordsService recordsService;

        public CategoriesService(ApplicationDbContext dbContext, IRecordsService recordsService)
        {
            this.dbContext = dbContext;
            this.recordsService = recordsService;
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

        public async Task<string> RemoveAsync(int oldCategoryId, int newCategoryId)
        {
            var oldCategory = await this.dbContext.Categories.FirstOrDefaultAsync(c => c.Id == oldCategoryId);

            if (oldCategory == null)
            {
                throw new ArgumentNullException(GlobalConstants.UnexistingCategory);
            }

            if (newCategoryId == -1)
            {
                decimal categoryAmount = 0;

                var categories = this.dbContext.Categories.Include(c => c.Records)
                    .Where(c => c.Id == oldCategoryId);

                foreach (var category in categories)
                {
                    foreach (var record in category.Records)
                    {
                        categoryAmount += record.Amount;
                    }
                }

                categoryAmount *= -1;

                var walletId = await this.dbContext.Categories
                    .Where(c => c.Id == oldCategoryId)
                    .Select(c => c.WalletId)
                    .FirstOrDefaultAsync();

                await this.recordsService.EditWalletAmountAsync(walletId, categoryAmount);

                await this.DeleteRecordsFromCategoryAsync(oldCategoryId);
            }
            else
            {
                var newCategory = await this.dbContext.Categories.FirstOrDefaultAsync(c => c.Id == newCategoryId);

                if (newCategory == null)
                {
                    throw new ArgumentNullException(GlobalConstants.UnexistingCategory);
                }

                await this.MoveCategoryOfRecordsAsync(oldCategoryId, newCategoryId);
            }

            var deletedCategoryName = oldCategory.Name;

            this.dbContext.Categories.Remove(oldCategory);
            await this.dbContext.SaveChangesAsync();

            return string.Format(GlobalConstants.SuccessfullyRemovedCategory, deletedCategoryName);
        }

        public async Task<string> EditAsync(int categoryId, string categoryName, string badgeColor)
        {
            var category = await this.dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                throw new ArgumentNullException(GlobalConstants.UnexistingCategory);
            }

            if (!Enum.TryParse<BadgeColor>(badgeColor, out BadgeColor badge))
            {
                throw new ArgumentException(GlobalConstants.BadgeColorNotValid);
            }

            category.BadgeColor = badge;
            category.ModifiedOn = DateTime.UtcNow;
            category.Name = categoryName;

            await this.dbContext.SaveChangesAsync();

            return GlobalConstants.CategorySuccessfullyUpdated;
        }

        public async Task<AllRecordsInCategoryDto> GetRecordsByKeywordAsync(string keyword, int categoryId, int page, int itemsPerPage = 12)
        {
            keyword = keyword.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                var allRecords = await this.GetRecordsByCategoryAsync(categoryId, page, itemsPerPage);

                return allRecords;
            }

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
                     Records = r.Records
                        .Where(r => r.Description.ToLower().Contains(keyword))
                        .OrderByDescending(x => x.CreatedOn)
                        .ThenBy(rec => rec.Amount)
                        .Skip((page - 1) * itemsPerPage)
                        .Take(itemsPerPage)
                        .Select(r => new CategoryRecordInfoDto
                         {
                             Id = r.Id,
                             Amount = r.Amount,
                             CreatedOn = r.CreatedOn,
                             Description = r.Description,
                             Type = r.Type,
                         })
                     .ToList(),
                 })
                 .FirstOrDefaultAsync();

            return categ;
        }

        public async Task<AllRecordsInCategoryDto> GetRecordsByDateRangeAsync(DateTime startDate, DateTime endDate, int categoryId, int page, int itemsPerPage = 12)
        {
            var startDate12AM = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var endDate12PM = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

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
                    Records = r.Records
                       .Where(r => r.CreatedOn >= startDate12AM && r.CreatedOn <= endDate12PM)
                       .OrderByDescending(x => x.CreatedOn)
                       .ThenBy(rec => rec.Amount)
                       .Skip((page - 1) * itemsPerPage)
                       .Take(itemsPerPage)
                       .Select(r => new CategoryRecordInfoDto
                       {
                           Id = r.Id,
                           Amount = r.Amount,
                           CreatedOn = r.CreatedOn,
                           Description = r.Description,
                           Type = r.Type,
                       })
                    .ToList(),
                })
                .FirstOrDefaultAsync();

            return categ;
        }

        public async Task<AllRecordsInCategoryDto> GetRecordsByCategoryAsync(int categoryId, int page, int itemsPerPage = 12)
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
                     Records = r.Records
                        .OrderByDescending(x => x.CreatedOn)
                        .ThenBy(rec => rec.Amount)
                        .Skip((page - 1) * itemsPerPage)
                        .Take(itemsPerPage)
                        .Select(r => new CategoryRecordInfoDto
                         {
                             Id = r.Id,
                             Amount = r.Amount,
                             CreatedOn = r.CreatedOn,
                             Description = r.Description,
                             Type = r.Type,
                         })
                     .ToList(),
                 })
                 .FirstOrDefaultAsync();

            return categ;
        }

        public int GetRecordsCount(int categoryId)
        {
            return this.dbContext.Records
                .Where(x => x.CategoryId == categoryId)
                .Count();
        }

        public int GetSearchRecordsCount(string searchTerm, int id)
        {
            string keyword = searchTerm.ToLower().Trim();

            return this.dbContext.Records
               .Where(x => x.CategoryId == id && x.Description.ToLower().Contains(keyword))
               .Count();
        }

        public int GetDateSortedRecordsCount(DateTime startDate, DateTime endDate, int categoryId)
        {
            var startDate12AM = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var endDate12PM = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            int count = this.dbContext.Records
                .Where(r => r.CreatedOn >= startDate12AM && r.CreatedOn <= endDate12PM && r.CategoryId == categoryId)
                .Count();

            return count;
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

        public async Task<DeleteCategoryDto> GetCategoryInfoForDeleteAsync(int categoryId, int walletId)
        {
            var model = await this.dbContext.Categories
                .Where(c => c.Id == categoryId)
                .Select(c => new DeleteCategoryDto
                {
                    WalletId = c.WalletId,
                    WalletName = c.Wallet.Name,
                    OldCategoryId = c.Id,
                    OldCategoryName = c.Name,
                    OldCategoryBadgeColor = c.BadgeColor,
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                throw new ArgumentException(GlobalConstants.UnexistingCategory);
            }

            var categories = await this.dbContext.Categories
                .Where(c => c.WalletId == walletId)
                .Select(c => new DeleteCategoryNameAndIdDto
                {
                    BadgeColor = c.BadgeColor,
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                })
                .ToListAsync();

            model.Categories = categories;

            return model;
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

        public async Task<bool> IsUserOwnCategoryAsync(string userId, int categoryId)
        {
            var category = await this.dbContext.Categories
                .Where(c => c.Id == categoryId && c.Wallet.ApplicationUserId == userId)
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> IsCategoryExistAsync(int walletId, string categoryName)
        {
            return await this.dbContext.Categories
                .AnyAsync(cat => cat.WalletId == walletId && cat.Name.ToLower() == categoryName.ToLower());
        }

        private async Task MoveCategoryOfRecordsAsync(int oldCategoryId, int newCategoryId)
        {
            var records = await this.dbContext.Records.Where(x => x.CategoryId == oldCategoryId).ToListAsync();

            for (int i = 0; i < records.Count(); i++)
            {
                records[i].CategoryId = newCategoryId;
            }

            await this.dbContext.SaveChangesAsync();
        }

        private async Task DeleteRecordsFromCategoryAsync(int categoryId)
        {
            var records = await this.dbContext.Records.Where(x => x.CategoryId == categoryId).ToListAsync();

            this.dbContext.RemoveRange(records);

            await this.dbContext.SaveChangesAsync();
        }
    }
}

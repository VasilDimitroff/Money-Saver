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

    public class WalletsService : IWalletsService
    {
        private readonly ApplicationDbContext dbContext;

        public WalletsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> AddAsync(string userId, string name, decimal initialMoney, string currencyName)
        {
            Wallet wallet = await this.GetWalletAsync(userId, name);

            if (wallet != null)
            {
                throw new ArgumentException(GlobalConstants.WalletAlreadyExist);
            }

            Currency currency = await this.GetCurrencyAsync(currencyName);

            if (currency == null)
            {
                throw new ArgumentException(GlobalConstants.InvalidCurrency);
            }

            string amountAsString = initialMoney.ToString("f2");
            initialMoney = decimal.Parse(amountAsString);

            Wallet newWallet = new Wallet
            {
                ApplicationUserId = userId,
                Name = name,
                MoneyAmount = initialMoney,
                Currency = currency,
            };

            await this.dbContext.AddAsync(newWallet);

            await this.dbContext.SaveChangesAsync();

            return string.Format(GlobalConstants.WalletSuccessfullyAdded, newWallet.Name);
        }

        public async Task<WalletCategoriesDto> GetWalletWithCategoriesAsync(int walletId)
        {
            // EF Core can't translate the query for total amount, total expenses and total incomes
            //decimal total = 0;

            var targetWallet = await this.dbContext.Wallets.Include(x => x.Categories).ThenInclude(x => x.Records)
                .FirstOrDefaultAsync(x => x.Id == walletId);

            // foreach (var item in wallets)
            // {
            //    foreach (var cat in item.Categories)
            //    {
            //        foreach (var rec in cat.Records)
            //        {
            //           total += rec.Amount;
            //        }
            //    }
            // }

            Record lastRecord = await this.dbContext.Records
                .OrderByDescending(r => r.CreatedOn)
                .FirstOrDefaultAsync();

            var wallet = await this.dbContext.Wallets
                .Where(w => w.Id == walletId)
                .Select(w => new WalletCategoriesDto
                {
                    ModifiedOn = lastRecord == null ? null : lastRecord.CreatedOn,
                    Currency = w.Currency.Code,
                    TotalAmount = targetWallet.MoneyAmount,
                    WalletName = w.Name,
                    WalletId = w.Id,
                    Categories = w.Categories.Select(c => new CategoryWalletInfoDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        RecordsCount = c.Records.Count(),
                        TotalIncomesAmount = c.Records.Where(r => r.Type == RecordType.Income).Sum(r => r.Amount),
                        TotalExpensesAmount = c.Records.Where(r => r.Type == RecordType.Expense).Sum(r => r.Amount),
                    })
                    .ToList(),
                })
                .FirstOrDefaultAsync();

            wallet.Incomes = wallet.Categories.Sum(x => x.TotalIncomesAmount);
            wallet.Outcomes = wallet.Categories.Sum(x => x.TotalExpensesAmount);

            return wallet;
        }

        public async Task<IEnumerable<WalletCategoriesDto>> GetWalletsAsync(string userId)
        {
            var wallets = await this.dbContext.Wallets
                .Where(w => w.ApplicationUserId == userId)
                .Select(w => new WalletCategoriesDto
                {
                    TotalAmount = decimal.Parse(w.Categories.Sum(x => x.Records.Sum(y => y.Amount)).ToString("f2")),
                    WalletName = w.Name,
                    WalletId = w.Id,
                    Categories = w.Categories.Select(c => new CategoryWalletInfoDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        RecordsCount = c.Records.Count(),
                    }),
                })
                .ToListAsync();

            return wallets;
        }

        // TODO: ID WE HAVE BUDGETS, MUST DELETE THEM IN THIS METHOD!

        public async Task<string> RemoveAsync(int walletId)
        {
            var categories = await this.dbContext.Categories
                .Where(c => c.WalletId == walletId)
                .ToListAsync();

            foreach (var category in categories)
            {
                var records = category.Records.ToList();

                this.dbContext.Records.RemoveRange(records);

                this.dbContext.Categories.Remove(category);
            }

            var walletForDelete = await this.dbContext.Wallets.FirstOrDefaultAsync(x => x.Id == walletId);

            this.dbContext.Remove(walletForDelete);

            await this.dbContext.SaveChangesAsync();

            return string.Format(GlobalConstants.WalletSuccessfullyRemoved, walletForDelete.Name);
        }

        public async Task<string> GetWalletNameAsync(int walletId)
        {
            var wallet = await this.dbContext.Wallets.FirstOrDefaultAsync(x => x.Id == walletId);

            if (wallet == null)
            {
                throw new ArgumentException(GlobalConstants.WalletNotExist);
            }

            return wallet.Name;
        }

        public async Task<WalletDetailsDto> GetWalletDetailsAsync(int walletId)
        {
            WalletDetailsDto targetWallet = await this.dbContext.Wallets
                .Where(w => w.Id == walletId)
                .Select(w => new WalletDetailsDto
                {
                    Currency = w.Currency.Code,
                    CurrentBalance = w.MoneyAmount,
                    WalletId = w.Id,
                    WalletName = w.Name,
                    Categories = w.Categories.Select(c => new WalletDetailsCategoryDto
                    {
                        CategoryId = c.Id,
                        CategoryName = c.Name,
                        TotalExpenses = c.Records.Where(r => r.Type == RecordType.Expense).Sum(r => r.Amount),
                        TotalIncomes = c.Records.Where(r => r.Type == RecordType.Income).Sum(r => r.Amount),
                        RecordsCount = c.Records.Count(),
                    }),
                })
                .FirstOrDefaultAsync();

            decimal totalWalletExpensesLast30days = this.dbContext.Records
                .Where(r => r.Category.WalletId == walletId && r.Type == RecordType.Expense && r.CreatedOn <= DateTime.UtcNow && r.CreatedOn >= DateTime.UtcNow.AddDays(-30))
                .Sum(r => r.Amount);

            decimal totalWalletIncomesLast30days = this.dbContext.Records
               .Where(r => r.Category.WalletId == walletId && r.Type == RecordType.Income && r.CreatedOn <= DateTime.UtcNow && r.CreatedOn >= DateTime.UtcNow.AddDays(-30))
               .Sum(r => r.Amount);

            int totalRecordsCountLast30Days = this.dbContext.Records
                .Where(r => r.Category.WalletId == walletId && r.CreatedOn <= DateTime.UtcNow && r.CreatedOn >= DateTime.UtcNow.AddDays(-30))
                .Count();

            targetWallet.TotalRecordsCountLast30Days = totalRecordsCountLast30Days;
            targetWallet.TotalWalletExpensesLast30Days = totalWalletExpensesLast30days;
            targetWallet.TotalWalletIncomesLast30Days = totalWalletIncomesLast30days;

            targetWallet.Records = await this.dbContext.Records
                .Where(r => r.Category.WalletId == walletId)
                .Select(r => new WalletDetailsRecordDto
                {
                    Id = r.Id,
                    Amount = r.Amount,
                    CategoryId = r.CategoryId,
                    CategoryName = r.Category.Name,
                    CreatedOn = r.CreatedOn,
                    Description = r.Description,
                })
                .ToListAsync();

            if (targetWallet == null)
            {
                throw new ArgumentNullException(GlobalConstants.WalletNotExist);
            }

            return targetWallet;
        }

        public async Task<int> GetWalletIdByRecordIdAsync(string recordId)
        {
            var wallet = await this.dbContext.Records
                .Where(x => x.Id == recordId)
                .Select(x => new WalletIdDto
                {
                    Id = x.Category.WalletId,
                })
                .FirstOrDefaultAsync();

            if (wallet == null)
            {
                throw new ArgumentException(GlobalConstants.WalletNotExist);
            }

            return wallet.Id;
        }

        public async Task<IEnumerable<CategoryBasicInfoDto>> GetWalletCategoriesAsync(int walletId)
        {
            var categories = await this.dbContext.Categories
                .Where(c => c.WalletId == walletId)
                .Select(c => new CategoryBasicInfoDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    WalletName = c.Wallet.Name,
                })
                .ToListAsync();

            return categories;
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByKeywordAsync(string keyword, int walletId)
        {
            keyword = keyword.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                var allRecords = await this.dbContext.Records
                 .Where(r => r.Category.WalletId == walletId)
                 .Select(r => new RecordInfoDto
                 {
                     Id = r.Id,
                     Amount = (r.Type == RecordType.Income) ? r.Amount : Math.Abs(r.Amount) * (-1),
                     Category = r.Category.Name,
                     CategoryId = r.CategoryId,
                     CreatedOn = r.CreatedOn,
                     Description = r.Description,
                     Type = r.Type.ToString(),
                     Wallet = r.Category.Wallet.Name,
                     Currency = r.Category.Wallet.Currency.Code,
                 })
                 .ToListAsync();

                return allRecords;
            }

            var records = await this.dbContext.Records
                 .Where(r => r.Description.Contains(keyword.ToLower()) && r.Category.WalletId == walletId)
                 .Select(r => new RecordInfoDto
                 {
                     Id = r.Id,
                     Amount = (r.Type == RecordType.Income) ? r.Amount : Math.Abs(r.Amount) * (-1),
                     Category = r.Category.Name,
                     CategoryId = r.CategoryId,
                     CreatedOn = r.CreatedOn,
                     Description = r.Description,
                     Type = r.Type.ToString(),
                     Wallet = r.Category.Wallet.Name,
                     Currency = r.Category.Wallet.Currency.Code,
                 })
                 .ToListAsync();

            return records;
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByDateRangeAsync(DateTime startDate, DateTime endDate, int walletId)
        {
            var records = await this.dbContext.Records
                 .Where(r => r.CreatedOn >= startDate && r.CreatedOn <= endDate && r.Category.WalletId == walletId)
                 .Select(r => new RecordInfoDto
                 {
                     Id = r.Id,
                     Amount = (r.Type == RecordType.Income) ? r.Amount : Math.Abs(r.Amount) * (-1),
                     Category = r.Category.Name,
                     CategoryId = r.CategoryId,
                     CreatedOn = r.CreatedOn,
                     Description = r.Description,
                     Type = r.Type.ToString(),
                     Wallet = r.Category.Wallet.Name,
                     Currency = r.Category.Wallet.Currency.Code,
                 })
                 .ToListAsync();

            return records;
        }

        private async Task<Wallet> GetWalletAsync(string userId, string walletName)
        {
            Wallet wallet = await this.dbContext.Wallets
                .FirstOrDefaultAsync(w => w.Name == walletName && w.ApplicationUserId == userId);

            return wallet;
        }

        private async Task<Currency> GetCurrencyAsync(string currencyName)
        {
           Currency currency = await this.dbContext.Currencies.FirstOrDefaultAsync(x => x.Name == currencyName);

           return currency;
        }
    }
}

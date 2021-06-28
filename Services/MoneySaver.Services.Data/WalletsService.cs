namespace MoneySaver.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using MoneySaver.Common;
    using MoneySaver.Data;
    using MoneySaver.Data.Common.Repositories;
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
        private readonly IRecordsService recordsService;
        private readonly ICurrenciesService currenciesService;
        private readonly ICategoriesService categoriesService;

        public WalletsService(
            ApplicationDbContext dbContext,
            IRecordsService recordsService,
            ICurrenciesService currenciesService,
            ICategoriesService categoriesService)
        {
            this.dbContext = dbContext;
            this.recordsService = recordsService;
            this.currenciesService = currenciesService;
            this.categoriesService = categoriesService;
        }

        public async Task<string> AddAsync(string userId, string name, decimal initialMoney, int currencyId)
        {
            Wallet wallet = await this.GetWalletAsync(userId, name);

            if (wallet != null)
            {
                throw new ArgumentException(GlobalConstants.WalletAlreadyExist);
            }

            Currency currency = await this.GetCurrencyAsync(currencyId);

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

            var wallets = await this.dbContext.Wallets.Where(w => w.ApplicationUserId == userId).ToListAsync();

            if (wallets.Count == 1)
            {
                await this.SeedWalletAsync(newWallet);
            }

            return string.Format(GlobalConstants.WalletSuccessfullyAdded, newWallet.Name);
        }

        public async Task<WalletCategoriesDto> GetWalletWithCategoriesAsync(int walletId)
        {
            // EF Core can't translate the query for total amount, total expenses and total incomes
            // decimal total = 0;
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
                        BadgeColor = c.BadgeColor.ToString(),
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
            var wallet = await this.dbContext
                .Wallets
                .Include(c => c.Categories)
                .ThenInclude(c => c.Records)
                .FirstOrDefaultAsync(x => x.Id == walletId);

            if (wallet == null)
            {
                throw new ArgumentNullException(GlobalConstants.WalletNotExist);
            }

            foreach (var category in wallet.Categories)
            {
                var records = category.Records.ToList();

                this.dbContext.Records.RemoveRange(records);

                this.dbContext.Categories.Remove(category);
            }

            this.dbContext.Remove(wallet);

            await this.dbContext.SaveChangesAsync();

            return string.Format(GlobalConstants.WalletSuccessfullyRemoved, wallet.Name);
        }

        public async Task EditAsync(string userId, int walletId, string name, decimal amount, int currencyId)
        {
            var model = new EditWalletDto();

            var currency = await this.dbContext.Currencies.FirstOrDefaultAsync(c => c.Id == currencyId);

            if (currency == null)
            {
                throw new ArgumentNullException(GlobalConstants.CurrencyNotExist);
            }

            var wallet = await this.dbContext.Wallets.FirstOrDefaultAsync(w => w.Id == walletId);
            wallet.ModifiedOn = DateTime.UtcNow;
            wallet.MoneyAmount = amount;
            wallet.Name = name;
            wallet.CurrencyId = currencyId;

            decimal sumOfRecordsAmount = 0m;

            var categories = this.dbContext.Categories.Include(c => c.Records)
               .Where(c => c.WalletId == walletId);

            foreach (var category in categories)
                {
                    foreach (var record in category.Records)
                    {
                        sumOfRecordsAmount += record.Amount;
                    }
                }

            await this.recordsService.EditWalletAmountAsync(walletId, sumOfRecordsAmount);

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<EditWalletDto> GetWalletInfoForEditAsync(string userId, int walletId)
        {
            var model = new EditWalletDto();

            var wallet = await this.dbContext.Wallets
                .Where(x => x.Id == walletId)
                .FirstOrDefaultAsync();

            var currency = await this.dbContext.Currencies.FirstOrDefaultAsync(c => c.Wallets.Any(x => x.Id == walletId));

            model.Currencies = await this.currenciesService.GetAllAsync();
            model.Amount = wallet.MoneyAmount;
            model.CurrencyId = wallet.CurrencyId;
            model.CurrentCurrencyCode = currency.Code;
            model.CurrentCurrencyName = currency.Name;
            model.Name = wallet.Name;
            model.Id = wallet.Id;

            return model;
        }

        public async Task<IEnumerable<AllWalletsDto>> GetAllWalletsAsync(string userId)
        {
            var wallets = await this.dbContext.Wallets
                .Include(w => w.Currency)
                .Include(w => w.Categories)
                .ThenInclude(c => c.Records)
                .Where(w => w.ApplicationUserId == userId)
                .ToListAsync();

            var walletsDto = new List<AllWalletsDto>();

            walletsDto = wallets.Select(w => new AllWalletsDto
                {
                    CurrentBalance = w.MoneyAmount,
                    WalletId = w.Id,
                    WalletName = w.Name,
                    TotalExpenses = w.Categories.Sum(c => c.Records.Where(r => r.Type == RecordType.Expense).Sum(r => r.Amount)),
                    TotalIncomes = w.Categories.Sum(c => c.Records.Where(r => r.Type == RecordType.Income).Sum(r => r.Amount)),
                    Currency = w.Currency.Code,
                })
                .ToList();

            return walletsDto;
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

        public async Task<WalletDetailsDto> GetWalletDetailsAsync(string userId, int walletId)
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
                    BadgeColor = c.BadgeColor,
                }),
            })
            .FirstOrDefaultAsync();

            if (targetWallet == null)
            {
                throw new ArgumentNullException(GlobalConstants.WalletNotExist);
            }

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
                    CategoryBadgeColor = r.Category.BadgeColor,
                })
                .ToListAsync();

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
                    BadgeColor = c.BadgeColor,
                })
                .ToListAsync();

            return categories;
        }

        public int GetCount(int walletId)
        {
            return this.dbContext.Records.Count(x => x.Category.WalletId == walletId);
        }

        public async Task<IEnumerable<RecordInfoDto>> GetAllRecordsAsync(int page, int walletId, int itemsPerPage = 12)
        {
            var wallet = await this.dbContext.Wallets.FirstOrDefaultAsync(w => w.Id == walletId);

            if (wallet == null)
            {
                throw new ArgumentException(GlobalConstants.WalletNotExist);
            }

            var records = await this.dbContext.Records
                 .Where(r => r.Category.WalletId == walletId)
                 .OrderByDescending(x => x.CreatedOn)
                 .Skip((page - 1) * itemsPerPage).Take(itemsPerPage)
                 .Select(r => new RecordInfoDto
                 {
                     Id = r.Id,
                     Amount = (r.Type == RecordType.Income) ? r.Amount : Math.Abs(r.Amount) * (-1),
                     Category = r.Category.Name,
                     CategoryId = r.CategoryId,
                     CreatedOn = r.CreatedOn,
                     ModifiedOn = r.ModifiedOn,
                     Description = r.Description,
                     Type = r.Type.ToString(),
                     Wallet = wallet.Name,
                     Currency = r.Category.Wallet.Currency.Code,
                     BadgeColor = r.Category.BadgeColor,
                 })
                 .OrderByDescending(x => x.CreatedOn)
                 .ToListAsync();

            return records;
        }

        public int GetSearchRecordsCount(string searchTerm, int walletId)
        {
            int count = this.dbContext.Records
                .Where(r => r.Description.ToLower().Contains(searchTerm.ToLower()) && r.Category.WalletId == walletId)
                .Count();
            return count;
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByKeywordAsync(string keyword, int walletId, int page, int itemsPerPage = 12)
        {
            keyword = keyword.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                var allRecords = await this.GetAllRecordsAsync(page, walletId, itemsPerPage);

                return allRecords;
            }

            var records = await this.dbContext.Records
                 .Where(r => r.Description.ToLower().Contains(keyword) && r.Category.WalletId == walletId)
                 .OrderByDescending(x => x.CreatedOn)
                 .Skip((page - 1) * itemsPerPage).Take(itemsPerPage)
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
                     BadgeColor = r.Category.BadgeColor,
                 })
                 .ToListAsync();

            return records;
        }

        public int GetDateSortedRecordsCount(DateTime startDate, DateTime endDate, int walletId)
        {
            var startDate12AM = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var endDate12PM = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            int count = this.dbContext.Records
                .Where(r => r.CreatedOn >= startDate12AM && r.CreatedOn <= endDate12PM && r.Category.WalletId == walletId)
                .Count();

            return count;
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByDateRangeAsync(DateTime startDate, DateTime endDate, int walletId, int page, int itemsPerPage = 12)
        {
            var startDate12AM = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var endDate12PM = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            // var startOfDay = date.Date;

           // var endOfDay = date.Date.AddHours(24);
            var records = await this.dbContext.Records
                 .Where(r => r.CreatedOn >= startDate12AM && r.CreatedOn <= endDate12PM && r.Category.WalletId == walletId)
                 .OrderByDescending(x => x.CreatedOn)
                 .Skip((page - 1) * itemsPerPage).Take(itemsPerPage)
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
                     BadgeColor = r.Category.BadgeColor,
                 })
                 .ToListAsync();

            return records;
        }

        public async Task<bool> IsUserOwnWalletAsync(string userId, int walletId)
        {
            var wallet = await this.dbContext.Wallets
                .Where(w => w.Id == walletId && w.ApplicationUserId == userId)
                .FirstOrDefaultAsync();

            if (wallet == null)
            {
                return false;
            }

            return true;
        }

        private async Task<Wallet> GetWalletAsync(string userId, string walletName)
        {
            Wallet wallet = await this.dbContext.Wallets
                .FirstOrDefaultAsync(w => w.Name == walletName && w.ApplicationUserId == userId);

            return wallet;
        }

        private async Task<Currency> GetCurrencyAsync(int currencyId)
        {
           Currency currency = await this.dbContext.Currencies.FirstOrDefaultAsync(x => x.Id == currencyId);

           return currency;
        }

        private async Task SeedWalletAsync(Wallet wallet)
        {
            await this.categoriesService.AddAsync("Alcohol", wallet.Id, "Danger");
            await this.categoriesService.AddAsync("Salary", wallet.Id, "Success");
            await this.categoriesService.AddAsync("Health", wallet.Id, "Warning");
            await this.categoriesService.AddAsync("Food", wallet.Id, "Info");
            await this.categoriesService.AddAsync("Electronics", wallet.Id, "Dark");
        }
    }
}

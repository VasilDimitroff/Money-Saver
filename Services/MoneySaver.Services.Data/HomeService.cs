namespace MoneySaver.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using MoneySaver.Data;
    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Services.Data.Models.Home;

    public class HomeService : IHomeService
    {
        private readonly ApplicationDbContext dbContext;

        public HomeService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IndexDto> GetIndexInfoAsync(string userId)
        {
            var indexDto = new IndexDto
            {
                CategoriesLast30DaysExpenses = await this.GetLast30DaysExpensesByCategoryAsync(userId),
                CategoriesLast30DaysIncomes = await this.GetLast30DaysIncomesByCategoryAsync(userId),
            };

            return indexDto;
        }

        private async Task<IEnumerable<AccountCategoryExpensesLast30DaysDto>> GetLast30DaysExpensesByCategoryAsync(string userId)
        {
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow;

            var categories = await this.dbContext.Categories
                .Include(c => c.Wallet)
                .ThenInclude(w => w.Currency)
                .Where(c => c.Wallet.ApplicationUserId == userId)
                .Select(c => new AccountCategoryExpensesLast30DaysDto
                {
                    CategoryId = c.Id,
                    BadgeColor = c.BadgeColor,
                    CategoryName = c.Name,
                    WalletId = c.WalletId,
                    CurrencyCode = c.Wallet.Currency.Code,
                    WalletName = c.Wallet.Name,
                    TotalExpenseRecordsLast30Days = c.Records
                        .Where(r => r.Type == RecordType.Expense && r.CreatedOn >= startDate && r.CreatedOn <= endDate)
                        .Count(),
                    TotalExpensesLast30Days = c.Records
                        .OrderByDescending(x => x.CreatedOn)
                        .Where(r => r.Type == RecordType.Expense && r.CreatedOn >= startDate && r.CreatedOn <= endDate)
                        .Sum(r => r.Amount),
                })
                .ToListAsync();

            return categories;
        }

        private async Task<IEnumerable<AccountCategoryIncomesLast30DaysDto>> GetLast30DaysIncomesByCategoryAsync(string userId)
        {
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow;

            var categories = await this.dbContext.Categories
                .Where(c => c.Wallet.ApplicationUserId == userId)
                .Include(c => c.Wallet)
                .ThenInclude(w => w.Currency)
                .Select(c => new AccountCategoryIncomesLast30DaysDto
                {
                    CategoryId = c.Id,
                    BadgeColor = c.BadgeColor,
                    CategoryName = c.Name,
                    WalletId = c.WalletId,
                    WalletName = c.Wallet.Name,
                    CurrencyCode = c.Wallet.Currency.Code,
                    TotalIncomeRecordsLast30Days = c.Records
                        .Where(r => r.Type == RecordType.Income && r.CreatedOn >= startDate && r.CreatedOn <= endDate)
                        .Count(),
                    TotalIncomesLast30Days = c.Records
                        .OrderByDescending(x => x.CreatedOn)
                        .Where(r => r.Type == RecordType.Income && r.CreatedOn >= startDate && r.CreatedOn <= endDate)
                        .Sum(r => r.Amount),
                })
                .ToListAsync();

            return categories;
        }
    }
}

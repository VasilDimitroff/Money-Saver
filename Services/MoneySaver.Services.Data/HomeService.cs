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
                ActiveToDoLists = await this.GetActiveListsAsync(userId),
                Wallets = await this.GetWalletsAsync(userId),
                InvestmentWallets = await this.GetInvestmentWalletsAsync(userId),
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

        private async Task<IEnumerable<IndexListDto>> GetActiveListsAsync(string userId)
        {
            var lists = await this.dbContext.ToDoLists
                .Include(l => l.ListItems)
                .Where(l => l.ApplicationUserId == userId && l.Status == StatusType.Active)
                .OrderBy(l => l.ListItems.OrderBy(li => li.ModifiedOn).FirstOrDefault().ModifiedOn)
                .ThenBy(l => l.ModifiedOn)
                .Select(l => new IndexListDto
                {
                    Id = l.Id,
                    Name = l.Name,
                })
                .ToListAsync();

            return lists;
        }

        private async Task<IEnumerable<IndexWalletDto>> GetWalletsAsync(string userId)
        {
            var wallets = await this.dbContext.Wallets
                .Where(w => w.ApplicationUserId == userId)
                .Select(w => new IndexWalletDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    CurrencyCode = w.Currency.Code,
                    Amount = w.MoneyAmount,
                })
                .ToListAsync();

            return wallets;
        }

        private async Task<IEnumerable<IndexInvestmentWalletDto>> GetInvestmentWalletsAsync(string userId)
        {
            var investmentWallets = await this.dbContext.InvestmentWallets
               .Where(iw => iw.ApplicationUserId == userId)
               .Select(iw => new IndexInvestmentWalletDto
               {
                   Id = iw.Id,
                   Name = iw.Name,
                   CurrencyCode = iw.Currency.Code,
                   TotalBuyTradesAmount = iw.Trades.Where(t => t.Type == TradeType.Buy).Sum(t => t.Price * t.StockQuantity),
                   TotalSellTradesAmount = iw.Trades.Where(t => t.Type == TradeType.Sell).Sum(t => t.Price * t.StockQuantity),
               })
               .ToListAsync();

            return investmentWallets;
        }
    }
}

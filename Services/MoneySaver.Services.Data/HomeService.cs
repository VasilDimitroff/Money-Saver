namespace MoneySaver.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using MoneySaver.Common;
    using MoneySaver.Data;
    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Services.Data.Models.Companies;
    using MoneySaver.Services.Data.Models.Currencies;
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
                AccountRecords = await this.GetLastRecordsAsync(userId, 10),
                AccountTrades = await this.GetLastTradesAsync(userId, 10),
                AccountHoldings = await this.GetHoldingsAsync(userId),
                AccountCategories = await this.GetCategoriesSummaryAsync(userId),
                TotalAccountExpenses = this.dbContext.Records
                 .Where(r => r.Category.Wallet.ApplicationUserId == userId && r.Type == RecordType.Expense)
                 .Count(),
                TotalAccountIncomes = this.dbContext.Records
                 .Where(r => r.Category.Wallet.ApplicationUserId == userId && r.Type == RecordType.Income)
                 .Count(),
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

        private async Task<IEnumerable<IndexRecordDto>> GetLastRecordsAsync(string userId, int count)
        {
            var records = await this.dbContext.Records
                .Where(r => r.Category.Wallet.ApplicationUserId == userId)
                .OrderByDescending(x => x.CreatedOn)
                .Take(count)
                .Select(r => new IndexRecordDto
                {
                    Id = r.Id,
                    Amount = Math.Round(r.Amount, 2),
                    CategoryName = r.Category.Name,
                    CategoryId = r.CategoryId,
                    CreatedOn = r.CreatedOn,
                    Description = r.Description,
                    Type = r.Type,
                    WalletName = r.Category.Wallet.Name,
                    WalletId = r.Category.WalletId,
                    CurrencyCode = r.Category.Wallet.Currency.Code,
                    CategoryBadgeColor = r.Category.BadgeColor,
                })
                .ToListAsync();

            return records;
        }

        private async Task<IEnumerable<IndexTradeDto>> GetLastTradesAsync(string userId, int count)
        {
            var trades = await this.dbContext.Trades
                .Where(t => t.InvestmentWallet.ApplicationUserId == userId)
                .OrderByDescending(t => t.CreatedOn)
                .Take(count)
                 .Select(t => new IndexTradeDto
                 {
                     Id = t.Id,
                     CreatedOn = t.CreatedOn,
                     Price = t.Price,
                     Type = t.Type,
                     StockQuantity = t.StockQuantity,
                     InvestmentWalletId = t.InvestmentWalletId,
                     InvestmentWalletName = t.InvestmentWallet.Name,
                     Company = new GetCompanyDto
                     {
                         Name = t.Company.Name,
                         Id = t.CompanyId,
                         Ticker = t.Company.Ticker,
                     },
                     Currency = new CurrencyInfoDto
                     {
                         Code = t.InvestmentWallet.Currency.Code,
                         CurrencyId = t.InvestmentWallet.CurrencyId,
                         Name = t.InvestmentWallet.Currency.Name,
                     },
                 })
                .ToListAsync();

            return trades;
        }

        private async Task<IEnumerable<IndexCompanyHoldingsDto>> GetHoldingsAsync(string userId)
        {
            var tradedCompanies = await this.dbContext.Companies
                .Where(c => c.Trades.Any(t => t.InvestmentWallet.ApplicationUserId == userId))
                .Include(c => c.Trades)
                .ThenInclude(t => t.InvestmentWallet)
                .ToListAsync();

            var companiesDto = new List<IndexCompanyHoldingsDto>();

            foreach (var company in tradedCompanies)
            {
                int totalSellCompanyQuantity = company.Trades
                    .Where(t => t.InvestmentWallet.ApplicationUserId == userId && t.Type == TradeType.Sell).Sum(t => t.StockQuantity);

                int totalBuyCompanyQuantity = company.Trades
                   .Where(t => t.InvestmentWallet.ApplicationUserId == userId && t.Type == TradeType.Buy).Sum(t => t.StockQuantity);

                int currentlyCompanyHoldings = totalBuyCompanyQuantity - totalSellCompanyQuantity;

                int buyTradeCompanyCount = company.Trades.Where(t => t.InvestmentWallet.ApplicationUserId == userId && t.Type == TradeType.Buy).Count();
                int sellTradeCompanyCount = company.Trades.Where(t => t.InvestmentWallet.ApplicationUserId == userId && t.Type == TradeType.Sell).Count();

                if (currentlyCompanyHoldings > 0)
                {
                    var companyHoldingDto = new IndexCompanyHoldingsDto
                    {
                        Name = company.Name,
                        Ticker = company.Ticker,
                        StocksHoldings = currentlyCompanyHoldings,
                        BuyTrades = buyTradeCompanyCount,
                        SellTrades = sellTradeCompanyCount,
                    };

                    companiesDto.Add(companyHoldingDto);
                }
            }

            return companiesDto;
        }

        private async Task<IEnumerable<IndexCategoriesSummaryDto>> GetCategoriesSummaryAsync(string userId)
        {
            var categories = await this.dbContext.Categories
                .Where(c => c.Wallet.ApplicationUserId == userId)
                .Select(c => new IndexCategoriesSummaryDto
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    BadgeColor = c.BadgeColor,
                    WalletId = c.WalletId,
                    WalletName = c.Wallet.Name,
                    RecordsCount = c.Records.Count(),
                    TotalExpenses = c.Records.Where(r => r.Type == RecordType.Expense).Sum(r => r.Amount),
                    TotalIncomes = c.Records.Where(r => r.Type == RecordType.Income).Sum(r => r.Amount),
                    CurrencyCode = c.Wallet.Currency.Code,
                })
                .ToListAsync();

            return categories;
        }
    }
}

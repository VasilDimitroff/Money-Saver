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

    public class TradesService : ITradesService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ICompaniesService companiesService;

        public TradesService(
            ApplicationDbContext dbContext,
            ICompaniesService companiesService
            )
        {
            this.dbContext = dbContext;
            this.companiesService = companiesService;
        }

        public async Task CreateBuyTradeAsync(string userId, string companyTicker, int quantity, decimal pricePerShare, int currencyId)
        {
            var company = await this.companiesService.GetCompanyByTickerAsync(companyTicker);

            if (pricePerShare > 0)
            {
                pricePerShare *= -1;
            }

            var userTrade = new UserTrade()
            {
                Id = Guid.NewGuid().ToString(),
                ApplicationUserId = userId,
                CreatedOn = DateTime.UtcNow,
                Company = company,
                CompanyTicker = company.Ticker,
                Price = pricePerShare,
                StockQuantity = quantity,
                Type = TradeType.Buy,
                CurrencyId = currencyId,
            };

            await this.dbContext.UsersTrades.AddAsync(userTrade);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task CreateSellTradeAsync(string userId, string companyTicker, int quantity, decimal pricePerShare, int currencyId)
        {
            var company = await this.companiesService.GetCompanyByTickerAsync(companyTicker);

            if (pricePerShare < 0)
            {
                pricePerShare *= -1;
            }

            int currentlyHoldings = this.GetCompanyStocksHoldingsCount(userId, companyTicker, quantity, currencyId);

            if (currentlyHoldings < quantity)
            {
                throw new ArgumentException(GlobalConstants.NotEnoughQuantity);
            }

            var userTrade = new UserTrade()
            {
                Id = Guid.NewGuid().ToString(),
                ApplicationUserId = userId,
                CreatedOn = DateTime.UtcNow,
                Company = company,
                CompanyTicker = company.Ticker.ToUpper(),
                Price = pricePerShare,
                StockQuantity = quantity,
                Type = TradeType.Sell,
                CurrencyId = currencyId,
            };

            await this.dbContext.UsersTrades.AddAsync(userTrade);
            await this.dbContext.SaveChangesAsync();
        }

        public int GetCompanyStocksHoldingsCount(string userId, string companyTicker, int quantity, int currencyId)
        {
            int totalBuyQuantity = this.dbContext.UsersTrades
                .Where(ut => ut.ApplicationUserId == userId
                    && ut.CompanyTicker == companyTicker
                    && ut.Type == TradeType.Buy
                    && ut.CurrencyId == currencyId)
                .Sum(ut => ut.StockQuantity);

            int totalSellQuantity = this.dbContext.UsersTrades
                .Where(ut => ut.ApplicationUserId == userId
                    && ut.CompanyTicker == companyTicker
                    && ut.Type == TradeType.Sell
                    && ut.CurrencyId == currencyId)
                .Sum(ut => ut.StockQuantity);

            int currentlyHoldingQuantity = totalBuyQuantity - totalSellQuantity;

            return currentlyHoldingQuantity;
        }

        private async Task<UserTrade> GetTrade(string userId, string companyTicker)
        {
            var userTrade = await this.dbContext.UsersTrades
                .FirstOrDefaultAsync(ut => ut.ApplicationUserId == userId && ut.CompanyTicker == companyTicker.ToUpper());

            if (userTrade == null)
            {
                throw new ArgumentException(GlobalConstants.UserIsNotHolder);
            }

            return userTrade;
        }
    }
}

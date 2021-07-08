namespace MoneySaver.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Data;
    using MoneySaver.Data.Models;
    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Services.Data.Contracts;

    public class TradesService : ITradesService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ICompaniesService companiesService;

        public TradesService(ApplicationDbContext dbContext, ICompaniesService companiesService)
        {
            this.dbContext = dbContext;
            this.companiesService = companiesService;
        }

        public async Task CreateBuyTradeAsync(string userId, string companyTicker, int quantity, decimal pricePerShare)
        {
            var company = await this.companiesService.GetCompanyByTickerAsync(companyTicker);

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
            };

            await this.dbContext.UsersTrades.AddAsync(userTrade);
            await this.dbContext.SaveChangesAsync();
        }
    }
}

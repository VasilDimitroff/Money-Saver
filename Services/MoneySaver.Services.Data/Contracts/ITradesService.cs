namespace MoneySaver.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models.InvestmentWallets;
    using MoneySaver.Services.Data.Models.Trades;

    public interface ITradesService
    {
        public Task CreateBuyTradeAsync(string userId, int investmentWalletId, string companyTicker, int quantity, decimal pricePerShare);

        public Task CreateSellTradeAsync(string userId, int investmentWalletId, string companyTicker, int quantity, decimal pricePerShare);

        public int GetCompanyStocksHoldingsCount(string companyTicker, int investmentWalletId);

        public Task<EditTradeDto> GetTradeInfoForEdit(string userId, string tradeId);

        public Task UpdateAsync(string userId, string tradeId, string companyTicker, int investmentWalletId, decimal price, int quantity, DateTime createdOn);

        public Task RemoveAsync(string userId, string tradeId);

        public Task<int> GetInvestmentWalletIdByTradeIdAsync(string tradeId);

        public Task<string> GetInvestmentWalletNameAsync(int investmentWalletId);
    }
}

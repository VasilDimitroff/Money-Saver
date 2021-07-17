namespace MoneySaver.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models.Trades;

    public interface ITradesService
    {
        public Task CreateBuyTradeAsync(string userId, int investmentWalletId, string companyTicker, int quantity, decimal pricePerShare);

        public Task CreateSellTradeAsync(string userId, int investmentWalletId, string companyTicker, int quantity, decimal pricePerShare);

        public int GetCompanyStocksHoldingsCount(string companyTicker, int quantity, int investmentWalletId);

        public Task<EditTradeDto> GetTradeInfoForEdit(string userId, string tradeId);
    }
}

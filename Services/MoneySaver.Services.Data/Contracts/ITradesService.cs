namespace MoneySaver.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITradesService
    {
        public Task CreateBuyTradeAsync(string userId, int investmentWalletId, string companyTicker, int quantity, decimal pricePerShare);

        public Task CreateSellTradeAsync(string userId, int investmentWalletId, string companyTicker, int quantity, decimal pricePerShare);

        public int GetCompanyStocksHoldingsCount(string companyTicker, int quantity, int investmentWalletId);
    }
}

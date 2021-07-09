namespace MoneySaver.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITradesService
    {
        public Task CreateBuyTradeAsync(string userId, string companyTicker, int quantity, decimal pricePerShare, int currencyId);

        public Task CreateSellTradeAsync(string userId, string companyTicker, int quantity, decimal pricePerShare, int currencyId);

        public int GetCompanyStocksHoldingsCount(string userId, string companyTicker, int quantity, int currencyId);
    }
}

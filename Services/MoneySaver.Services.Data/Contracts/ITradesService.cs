namespace MoneySaver.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITradesService
    {
        public Task CreateBuyTradeAsync(string userId, string companyTicker, int quantity, decimal pricePerShare);
    }
}

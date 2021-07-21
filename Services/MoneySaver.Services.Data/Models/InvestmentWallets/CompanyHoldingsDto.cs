namespace MoneySaver.Services.Data.Models.InvestmentWallets
{
    using MoneySaver.Services.Data.Models.Companies;

    public class CompanyHoldingsDto : GetCompanyDto
    {
        public int StocksHoldings { get; set; }

        public int BuyTrades { get; set; }

        public int SellTrades { get; set; }
    }
}

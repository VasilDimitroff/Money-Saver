namespace MoneySaver.Web.ViewModels.Investments
{
    using MoneySaver.Web.ViewModels.Trades;

    public class CompanyHoldingsViewModel : CompanyViewModel
    {
        public int StocksHoldings { get; set; }

        public int BuyTrades { get; set; }

        public int SellTrades { get; set; }
    }
}

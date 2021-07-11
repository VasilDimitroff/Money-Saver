namespace MoneySaver.Web.ViewModels.Investments
{
    using System.Collections.Generic;

    using MoneySaver.Web.ViewModels.Trades;

    public class InvestmentWalletTradesViewModel : InvestmentWalletViewModel
    {
        public InvestmentWalletTradesViewModel()
        {
            this.Trades = new HashSet<TradeViewModel>();
        }

        public IEnumerable<TradeViewModel> Trades { get; set; }
    }
}

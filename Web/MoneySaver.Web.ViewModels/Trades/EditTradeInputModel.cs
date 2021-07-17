namespace MoneySaver.Web.ViewModels.Trades
{
    using System;
    using System.Collections.Generic;

    using MoneySaver.Web.ViewModels.Investments;

    public class EditTradeInputModel : AddTradeInputModel
    {
        public EditTradeInputModel()
        {
            this.AllInvestmentWallets = new List<InvestmentWalletIdNameAndCurrencyViewModel>();
        }

        public string Id { get; set; }

        public InvestmentWalletIdNameAndCurrencyViewModel InvestmentWallet { get; set; }

        public IEnumerable<InvestmentWalletIdNameAndCurrencyViewModel> AllInvestmentWallets { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

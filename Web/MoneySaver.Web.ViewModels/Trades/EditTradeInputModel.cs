namespace MoneySaver.Web.ViewModels.Trades
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Web.ViewModels.Investments;

    public class EditTradeInputModel : AddTradeInputModel
    {
        public EditTradeInputModel()
        {
            this.AllInvestmentWallets = new List<InvestmentWalletIdNameAndCurrencyViewModel>();
        }

        [Required]
        public string Id { get; set; }

        public InvestmentWalletIdNameAndCurrencyViewModel InvestmentWallet { get; set; }

        public IEnumerable<InvestmentWalletIdNameAndCurrencyViewModel> AllInvestmentWallets { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

namespace MoneySaver.Web.ViewModels.Investments
{
    using System;
    using System.Collections.Generic;

    using MoneySaver.Web.ViewModels.Currencies;
    using MoneySaver.Web.ViewModels.Trades;

    public class InvestmentWalletViewModel : TradesPagingViewModel
    {
        public string Name { get; set; }

        public CurrencyViewModel Currency { get; set; }

        public DateTime CreatedOn { get; set; }

        public decimal TotalBuyTradesAmount { get; set; }

        public decimal TotalSellTradesAmount { get; set; }

        public decimal Amount => this.TotalBuyTradesAmount + this.TotalSellTradesAmount;

        public decimal TotalTradesCount { get; set; }

    }
}

namespace MoneySaver.Web.ViewModels.Home
{
    using System;

    public class IndexInvestmentWalletViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CurrencyCode { get; set; }

        public decimal TotalBuyTradesAmount { get; set; }

        public decimal TotalSellTradesAmount { get; set; }

        public decimal Amount => Math.Round(this.TotalBuyTradesAmount + this.TotalSellTradesAmount, 2);
    }
}

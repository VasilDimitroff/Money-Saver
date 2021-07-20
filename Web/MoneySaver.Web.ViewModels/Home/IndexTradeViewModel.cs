namespace MoneySaver.Web.ViewModels.Home
{
    using System;

    using MoneySaver.Web.ViewModels.Currencies;
    using MoneySaver.Web.ViewModels.Trades;
    using MoneySaver.Web.ViewModels.Trades.Enums;

    public class IndexTradeViewModel
    {
        public string Id { get; set; }

        public CompanyViewModel Company { get; set; }

        public CurrencyViewModel Currency { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedOn { get; set; }

        public int StockQuantity { get; set; }

        public TradeType Type { get; set; }

        public decimal TotalAmount => Math.Round(this.Price * this.StockQuantity, 2);

        public int InvestmentWalletId { get; set; }

        public string InvestmentWalletName { get; set; }
    }
}

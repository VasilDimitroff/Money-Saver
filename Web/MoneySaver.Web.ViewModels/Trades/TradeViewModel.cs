namespace MoneySaver.Web.ViewModels.Trades
{
    using System;

    using MoneySaver.Web.ViewModels.Trades.Enums;

    public class TradeViewModel
    {
        public string Id { get; set; }

        public CompanyViewModel Company { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedOn { get; set; }

        public int StockQuantity { get; set; }

        public TradeType Type { get; set; }

        public decimal TotalAmount => Math.Round(this.Price * this.StockQuantity);

        public int InvestmentWalletId { get; set; }
    }
}

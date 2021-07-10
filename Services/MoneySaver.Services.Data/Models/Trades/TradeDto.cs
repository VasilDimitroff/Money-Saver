namespace MoneySaver.Services.Data.Models.Trades
{
    using System;

    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Services.Data.Models.Companies;

    public class TradeDto
    {
        public string Id { get; set; }

        public GetCompanyDto Company { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedOn { get; set; }

        public int StockQuantity { get; set; }

        public TradeType Type { get; set; }

        public int InvestmentWalletId { get; set; }
    }
}

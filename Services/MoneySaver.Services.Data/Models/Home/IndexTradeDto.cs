namespace MoneySaver.Services.Data.Models.Home
{
    using System;

    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Services.Data.Models.Companies;
    using MoneySaver.Services.Data.Models.Currencies;

    public class IndexTradeDto
    {
        public string Id { get; set; }

        public GetCompanyDto Company { get; set; }

        public CurrencyInfoDto Currency { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedOn { get; set; }

        public int StockQuantity { get; set; }

        public TradeType Type { get; set; }

        public int InvestmentWalletId { get; set; }

        public string InvestmentWalletName { get; set; }
    }
}

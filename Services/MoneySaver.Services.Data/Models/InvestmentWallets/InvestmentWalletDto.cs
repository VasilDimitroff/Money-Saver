namespace MoneySaver.Services.Data.Models.InvestmentWallets
{
    using System;
    using System.Collections.Generic;

    using MoneySaver.Services.Data.Models.Currencies;
    using MoneySaver.Services.Data.Models.Trades;

    public class InvestmentWalletDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public CurrencyInfoDto Currency { get; set; }

        public DateTime CreatedOn { get; set; }

        public decimal TotalBuyTradesAmount { get; set; }

        public decimal TotalTradesCount { get; set; }

        public decimal TotalSellTradesAmount { get; set; }
    }
}

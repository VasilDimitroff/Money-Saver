namespace MoneySaver.Services.Data.Models.InvestmentWallets
{
    using System;
    using System.Collections.Generic;

    using MoneySaver.Services.Data.Models.Trades;

    public class InvestmentWalletTradesDto : InvestmentWalletDto
    {
        public InvestmentWalletTradesDto()
        {
            this.Trades = new HashSet<TradeDto>();
        }

        public IEnumerable<TradeDto> Trades { get; set; }
    }
}

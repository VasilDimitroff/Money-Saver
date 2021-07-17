namespace MoneySaver.Services.Data.Models.Trades
{
    using System;
    using System.Collections.Generic;

    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Services.Data.Models.Companies;
    using MoneySaver.Services.Data.Models.InvestmentWallets;

    public class EditTradeDto
    {
        public EditTradeDto()
        {
            this.AllInvestmentWallets = new List<InvestmentWalletIdNameAndCurrencyDto>();
        }

        public string Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public InvestmentWalletIdNameAndCurrencyDto InvestmentWallet { get; set; }

        public IEnumerable<InvestmentWalletIdNameAndCurrencyDto> AllInvestmentWallets { get; set; }

        public TradeType Type { get; set; }

        public GetCompanyDto SelectedCompany { get; set; }
    }
}

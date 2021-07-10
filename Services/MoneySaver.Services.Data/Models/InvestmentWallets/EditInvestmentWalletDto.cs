namespace MoneySaver.Services.Data.Models.InvestmentWallets
{
    using System.Collections.Generic;

    using MoneySaver.Services.Data.Models.Currencies;

    public class EditInvestmentWalletDto
    {
        public EditInvestmentWalletDto()
        {
            this.Currencies = new HashSet<CurrencyInfoDto>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public CurrencyInfoDto SelectedCurrency { get; set; }

        public IEnumerable<CurrencyInfoDto> Currencies { get; set; }
    }
}

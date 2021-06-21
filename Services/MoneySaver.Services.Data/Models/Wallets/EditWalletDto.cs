namespace MoneySaver.Services.Data.Models.Wallets
{
    using System.Collections.Generic;

    using MoneySaver.Services.Data.Models.Currencies;


    public class EditWalletDto
    {
        public EditWalletDto()
        {
            this.Currencies = new HashSet<CurrencyInfoDto>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int CurrencyId { get; set; }

        public string CurrentCurrencyCode { get; set; }

        public string CurrentCurrencyName { get; set; }

        public decimal Amount { get; set; }

        public IEnumerable<CurrencyInfoDto> Currencies { get; set; }
    }
}

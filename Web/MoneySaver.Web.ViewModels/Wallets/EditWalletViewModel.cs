namespace MoneySaver.Web.ViewModels.Wallets
{
    using System.Collections.Generic;

    using MoneySaver.Web.ViewModels.Currencies;

    public class EditWalletViewModel
    {
        public EditWalletViewModel()
        {
            this.Currencies = new HashSet<CurrencyViewModel>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int CurrencyId { get; set; }

        public string CurrentCurrencyCode { get; set; }

        public string CurrentCurrencyName { get; set; }

        public decimal Amount { get; set; }

        public IEnumerable<CurrencyViewModel> Currencies { get; set; }
    }
}

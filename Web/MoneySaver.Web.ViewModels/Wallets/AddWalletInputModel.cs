namespace MoneySaver.Web.ViewModels.Wallets
{
    using System.Collections.Generic;

    using MoneySaver.Web.ViewModels.Currencies;

    public class AddWalletInputModel
    {
        public AddWalletInputModel()
        {
            this.Currencies = new HashSet<CurrencyViewModel>();
        }

        public string Name { get; set; }

        public int CurrencyId { get; set; }

        public decimal Amount { get; set; }

        public string ApplicationUserId { get; set; }

        public IEnumerable<CurrencyViewModel> Currencies { get; set; }
    }
}

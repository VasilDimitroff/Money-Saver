namespace MoneySaver.Web.ViewModels.Investments
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Web.ViewModels.Currencies;

    public class AddInvestmentWalletInputModel
    {
        public AddInvestmentWalletInputModel()
        {
            this.Currencies = new HashSet<CurrencyViewModel>();
        }

        public string Name { get; set; }

        [Required(ErrorMessage ="Please select a currency first")]
        [Range(1, 1000000)]
        public int SelectedCurrencyId { get; set; }

        public IEnumerable<CurrencyViewModel> Currencies { get; set; }
    }
}

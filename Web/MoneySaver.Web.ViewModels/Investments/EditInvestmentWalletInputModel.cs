namespace MoneySaver.Web.ViewModels.Investments
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Web.ViewModels.Currencies;

    public class EditInvestmentWalletInputModel
    {
        public EditInvestmentWalletInputModel()
        {
            this.Currencies = new HashSet<CurrencyViewModel>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        [Required]
        public CurrencyViewModel SelectedCurrency { get; set; }

        public IEnumerable<CurrencyViewModel> Currencies { get; set; }
    }
}

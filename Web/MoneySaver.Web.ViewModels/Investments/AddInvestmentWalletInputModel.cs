namespace MoneySaver.Web.ViewModels.Investments
{
    using MoneySaver.Web.ViewModels.Currencies;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AddInvestmentWalletInputModel
    {
        public AddInvestmentWalletInputModel()
        {


        }

        public string Name { get; set; }

        [Required]
        [Range(1, 1000000)]
        public int SelectedCurrencyId { get; set; }

        public IEnumerable<CurrencyViewModel> Currencies { get; set; }
    }
}

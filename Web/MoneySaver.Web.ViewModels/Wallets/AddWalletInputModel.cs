namespace MoneySaver.Web.ViewModels.Wallets
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Web.ViewModels.Currencies;

    public class AddWalletInputModel
    {
        public AddWalletInputModel()
        {
            this.Currencies = new HashSet<CurrencyViewModel>();
        }

        [Required(ErrorMessage = "Wallet Name is required")]
        [MinLength(1)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public IEnumerable<CurrencyViewModel> Currencies { get; set; }
    }
}

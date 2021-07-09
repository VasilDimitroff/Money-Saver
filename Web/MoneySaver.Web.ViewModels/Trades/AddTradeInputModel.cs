namespace MoneySaver.Web.ViewModels.Trades
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Web.ViewModels.Currencies;
    using MoneySaver.Web.ViewModels.Trades.Enums;

    public class AddTradeInputModel
    {
        public AddTradeInputModel()
        {
            this.Companies = new HashSet<CompanyViewModel>();
            this.Currencies = new HashSet<CurrencyViewModel>();
        }

        [Range(1, 1000000)]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 1000000)]
        public TradeType Type { get; set; }

        public CompanyViewModel SelectedCompany { get; set; }

        [Required]
        [Range(1, 1000000)]
        public int SelectedCurrencyId { get; set; }

        public decimal Amount => this.Price * this.Quantity;

        public ICollection<CompanyViewModel> Companies { get; set; }

        public IEnumerable<CurrencyViewModel> Currencies { get; set; }
    }
}

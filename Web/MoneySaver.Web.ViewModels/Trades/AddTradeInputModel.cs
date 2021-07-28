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
        }

        [Range(1, 10000000, ErrorMessage = "Quantity should be between 1 and 10 000 000")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price per share is required")]
        public decimal Price { get; set; }

        [Required]
        public int InvestmentWalletId { get; set; }

        public string InvestmentWalletName { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "Please choose Trade Type")]
        public TradeType Type { get; set; }

        public CompanyViewModel SelectedCompany { get; set; }

        public decimal Amount => this.Price * this.Quantity;

        public ICollection<CompanyViewModel> Companies { get; set; }
    }
}

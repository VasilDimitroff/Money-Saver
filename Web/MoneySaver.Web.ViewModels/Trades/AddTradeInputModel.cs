namespace MoneySaver.Web.ViewModels.Trades
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using MoneySaver.Web.ViewModels.Trades.Enums;

    public class AddTradeInputModel
    {
        public AddTradeInputModel()
        {
            this.Companies = new HashSet<CompanyViewModel>();
        }

        [Range(1, 1000000)]
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public TradeType Type { get; set; }

        public CompanyViewModel SelectedCompany { get; set; }

        public decimal Amount => this.Price * this.Quantity;

        public ICollection<CompanyViewModel> Companies { get; set; }
    }
}

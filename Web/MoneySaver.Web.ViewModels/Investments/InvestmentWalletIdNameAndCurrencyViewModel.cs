using System.ComponentModel.DataAnnotations;

namespace MoneySaver.Web.ViewModels.Investments
{
    public class InvestmentWalletIdNameAndCurrencyViewModel
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }

        public string CurrencyCode { get; set; }
    }
}

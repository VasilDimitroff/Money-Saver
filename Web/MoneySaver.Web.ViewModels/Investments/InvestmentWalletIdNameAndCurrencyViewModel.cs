namespace MoneySaver.Web.ViewModels.Investments
{
    using System.ComponentModel.DataAnnotations;

    public class InvestmentWalletIdNameAndCurrencyViewModel
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }

        public string CurrencyCode { get; set; }
    }
}

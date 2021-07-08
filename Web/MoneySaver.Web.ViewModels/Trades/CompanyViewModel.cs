namespace MoneySaver.Web.ViewModels.Trades
{
    using System.ComponentModel.DataAnnotations;

    public class CompanyViewModel
    {
        [Required]
        public string Ticker { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
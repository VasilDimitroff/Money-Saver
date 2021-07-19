namespace MoneySaver.Web.ViewModels.Trades
{
    using System.ComponentModel.DataAnnotations;

    public class CompanyViewModel
    {
        [Required]
        public string Ticker { get; set; }

        public string Name { get; set; }
    }
}

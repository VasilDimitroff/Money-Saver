namespace MoneySaver.Web.ViewModels.Trades
{
    using System.ComponentModel.DataAnnotations;

    public class CompanyViewModel
    {
        [Required]
        public string Id { get; set; }

        public string Ticker { get; set; }

        public string Name { get; set; }
    }
}

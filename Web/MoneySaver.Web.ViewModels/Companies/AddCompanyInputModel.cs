namespace MoneySaver.Web.ViewModels.Companies
{
    using System.ComponentModel.DataAnnotations;

    public class AddCompanyInputModel
    {
        [Required]
        public string Ticker { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Range(1, int.MaxValue)]
        public int InvestmentWalletId { get; set; }
    }
}

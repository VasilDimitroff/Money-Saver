namespace MoneySaver.Web.ViewModels.Companies
{
    using System.ComponentModel.DataAnnotations;

    public class AddCompanyInputModel
    {
        [Required(ErrorMessage = "Please enter a company ticker")]
        [MaxLength(10)]
        public string Ticker { get; set; }

        [Required(ErrorMessage = "Please enter a company name")]
        [MaxLength(50)]
        public string CompanyName { get; set; }

        public int InvestmentWalletId { get; set; }
    }
}

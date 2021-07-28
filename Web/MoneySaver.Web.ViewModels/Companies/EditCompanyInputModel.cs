namespace MoneySaver.Web.ViewModels.Companies
{
    using System.ComponentModel.DataAnnotations;

    public class EditCompanyInputModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(10)]
        public string Ticker { get; set; }
    }
}

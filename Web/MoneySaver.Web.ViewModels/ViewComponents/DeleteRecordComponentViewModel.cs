namespace MoneySaver.Web.ViewModels.ViewComponents
{
    using System.ComponentModel.DataAnnotations;

    public class DeleteRecordComponentViewModel
    {
        [Required]
        public string Id { get; set; }
    }
}

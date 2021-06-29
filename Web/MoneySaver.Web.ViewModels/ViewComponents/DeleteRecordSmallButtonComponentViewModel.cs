namespace MoneySaver.Web.ViewModels.ViewComponents
{
    using System.ComponentModel.DataAnnotations;

    public class DeleteRecordSmallButtonComponentViewModel
    {
        [Required]
        public string RecordId { get; set; }
    }
}

namespace MoneySaver.Web.ViewModels.ViewComponents
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class GetRecordsInCategoryByDateComponentViewModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; }

        public DateTime DefaultStartDate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        public DateTime EndDate { get; set; }

        public DateTime DefaultEndDate { get; set; }
    }
}

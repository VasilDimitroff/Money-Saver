namespace MoneySaver.Web.ViewModels.Records
{
    using System;

    using System.ComponentModel.DataAnnotations;

    public class EditRecordViewModel : AddRecordViewModel
    {
        [Required]
        public string Id { get; set; }

        public decimal OldAmount { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}

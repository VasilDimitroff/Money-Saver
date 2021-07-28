namespace MoneySaver.Web.ViewModels.Records
{
    using System;

    using System.ComponentModel.DataAnnotations;

    public class EditRecordInputModel : AddRecordViewModel
    {
        [Required(ErrorMessage = "Record Id is required")]
        public string Id { get; set; }

        public decimal OldAmount { get; set; }
    }
}

namespace MoneySaver.Web.ViewModels.Records
{
    using System;

    using System.ComponentModel.DataAnnotations;

    public class EditRecordInputModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Description cannot be empty")]
        [MinLength(1)]
        [MaxLength(250)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category Id is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Wallet Id is required")]
        public int WalletId { get; set; }

        [Required(ErrorMessage = "Type of category is required")]
        public string Type { get; set; }

        public decimal OldAmount { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

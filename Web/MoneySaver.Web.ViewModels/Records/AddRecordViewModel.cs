namespace MoneySaver.Web.ViewModels.Records
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Web.ViewModels.Categories;
    using MoneySaver.Web.ViewModels.Records.Enums;
    using MoneySaver.Web.ViewModels.Wallets;

    public class AddRecordViewModel
    {
        [Required(ErrorMessage = "Wallet Id is required")]
        public int WalletId { get; set; }

        [Required(ErrorMessage = "Category Id is required")]
        public int CategoryId { get; set; }

        public string Description { get; set; }

        public string WalletName { get; set; }

        [Required(ErrorMessage = "Please enter amount")]
        public decimal Amount { get; set; }

        public DateTime? CreatedOn { get; set; }

        public IEnumerable<CategoryNameIdViewModel> Categories { get; set; }

        [Required]
        public RecordTypeInputModel Type { get; set; }
    }
}

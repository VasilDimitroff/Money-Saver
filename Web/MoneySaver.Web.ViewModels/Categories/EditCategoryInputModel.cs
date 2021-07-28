namespace MoneySaver.Web.ViewModels.Categories
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Web.ViewModels.Records.Enums;

    public class EditCategoryInputModel
    {
        public EditCategoryInputModel()
        {
            this.Wallets = new HashSet<EditCategoryWalletsListViewModel>();
        }

        [Required]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [MinLength(1)]
        [MaxLength(25)]
        public string CategoryName { get; set; }

        [Required]
        public int WalletId { get; set; }

        public string WalletName { get; set; }

        [Required(ErrorMessage = "Badge Color is required")]
        public BadgeColor BadgeColor { get; set; }

        public IEnumerable<EditCategoryWalletsListViewModel> Wallets { get; set; }
    }
}

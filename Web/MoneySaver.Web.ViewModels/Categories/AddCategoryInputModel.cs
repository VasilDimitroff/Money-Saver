namespace MoneySaver.Web.ViewModels.Categories
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AddCategoryInputModel
    {
        public AddCategoryInputModel()
        {
            this.Wallets = new HashSet<AddCategoryWalletsListViewModel>();
        }

        [Required(ErrorMessage = "Category Name is required")]
        public string Name { get; set; }

        public string WalletName { get; set; }

        [Required]
        public int WalletId { get; set; }

        [Required(ErrorMessage = "Badge Color is required")]
        public string BadgeColor { get; set; }

        public IEnumerable<AddCategoryWalletsListViewModel> Wallets { get; set; }
    }
}

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

        [Required]
        public string CategoryName { get; set; }

        [Required]
        public int WalletId { get; set; }

        [Required]
        public BadgeColor BadgeColor { get; set; }

        public IEnumerable<EditCategoryWalletsListViewModel> Wallets { get; set; }
    }
}

namespace MoneySaver.Web.ViewModels.Categories
{
    using System.Collections.Generic;

    using MoneySaver.Web.ViewModels.Records.Enums;

    public class EditCategoryInputModel
    {
        public EditCategoryInputModel()
        {
            this.Wallets = new HashSet<EditCategoryWalletsListViewModel>();
        }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int WalletId { get; set; }

        public BadgeColor BadgeColor { get; set; }

        public IEnumerable<EditCategoryWalletsListViewModel> Wallets { get; set; }
    }
}

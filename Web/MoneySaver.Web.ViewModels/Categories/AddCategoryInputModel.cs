namespace MoneySaver.Web.ViewModels.Categories
{
    using System.Collections.Generic;

    public class AddCategoryInputModel
    {
        public AddCategoryInputModel()
        {
            this.Wallets = new HashSet<AddCategoryWalletsListViewModel>();
        }

        public string Name { get; set; }

        public string WalletName { get; set; }

        public int WalletId { get; set; }

        public IEnumerable<AddCategoryWalletsListViewModel> Wallets { get; set; }
    }
}

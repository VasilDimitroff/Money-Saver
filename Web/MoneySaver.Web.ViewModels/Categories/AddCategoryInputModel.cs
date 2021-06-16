namespace MoneySaver.Web.ViewModels.Categories
{
    using System.Collections.Generic;

    public class AddCategoryInputModel
    {
        public AddCategoryInputModel()
        {
            this.Wallets = new HashSet<WalletNameAndIdViewModel>();
        }

        public string Name { get; set; }

        public int WalletId { get; set; }

        public IEnumerable<WalletNameAndIdViewModel> Wallets { get; set; }
    }
}

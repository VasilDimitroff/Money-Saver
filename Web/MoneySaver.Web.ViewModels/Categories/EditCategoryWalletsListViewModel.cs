namespace MoneySaver.Web.ViewModels.Categories
{
    using System.ComponentModel.DataAnnotations;

    public class EditCategoryWalletsListViewModel
    {
        [Required]
        public int WalletId { get; set; }

        public string WalletName { get; set; }
    }
}

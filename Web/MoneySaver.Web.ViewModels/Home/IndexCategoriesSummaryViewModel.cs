namespace MoneySaver.Web.ViewModels.Home
{
    using MoneySaver.Web.ViewModels.Wallets;

    public class IndexCategoriesSummaryViewModel : WalletDetailsCategoryViewModel
    {
        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public string CurrencyCode { get; set; }
    }
}

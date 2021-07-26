namespace MoneySaver.Services.Data.Models.Home
{
    using MoneySaver.Services.Data.Models.Wallets;

    public class IndexCategoriesSummaryDto : WalletDetailsCategoryDto
    {
        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public string CurrencyCode { get; set; }
    }
}

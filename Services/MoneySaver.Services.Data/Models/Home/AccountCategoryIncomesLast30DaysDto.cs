namespace MoneySaver.Services.Data.Models.Home
{
    using MoneySaver.Services.Data.Models.Wallets;

    public class AccountCategoryIncomesLast30DaysDto : CategoryIncomesLast30DaysWalletDetailsDto
    {
        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public string CurrencyCode { get; set; }
    }
}

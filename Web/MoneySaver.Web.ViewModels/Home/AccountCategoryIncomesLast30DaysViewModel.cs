namespace MoneySaver.Web.ViewModels.Home
{
    using System;

    using MoneySaver.Web.ViewModels.Wallets;

    public class AccountCategoryIncomesLast30DaysViewModel : CategoryIncomesLast30DaysWalletDetailsViewModel
    {
        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public string CurrencyCode { get; set; }
    }
}

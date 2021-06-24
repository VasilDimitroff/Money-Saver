namespace MoneySaver.Web.ViewModels.Wallets
{
    using System;
    using System.Collections.Generic;

    public class WalletSearchResultViewModel : SearchPagingViewModel
    {
        public string Wallet { get; set; }

        public IEnumerable<WalletSearchResultSingleRecordViewModel> Records { get; set; }
    }
}

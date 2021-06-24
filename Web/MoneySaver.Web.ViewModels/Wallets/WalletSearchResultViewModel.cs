namespace MoneySaver.Web.ViewModels.Wallets
{
    using System;
    using System.Collections.Generic;

    public class WalletSearchResultViewModel : SearchPagingViewModel
    {
        public string Wallet { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<WalletSearchResultSingleRecordViewModel> Records { get; set; }
    }
}

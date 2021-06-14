namespace MoneySaver.Web.ViewModels.Wallets
{
    using System;
    using System.Collections.Generic;

    public class WalletSearchResultViewModel
    {
        public int WalletId { get; set; }

        public string Wallet { get; set; }

        public string SearchTerm { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<WalletSearchResultSingleRecordViewModel> Records { get; set; }
    }
}

namespace MoneySaver.Web.Models.Wallets

{
    using System;
    using System.Collections.Generic;

    using MoneySaver.Web.Infrastructure.CustomValidations;

    public class WalletSearchResultViewModel
    {
        [IsWalletExist]
        public int WalletId { get; set; }

        public string Wallet { get; set; }

        public string SearchTerm { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<WalletSearchResultSingleRecordViewModel> Records { get; set; }
    }
}

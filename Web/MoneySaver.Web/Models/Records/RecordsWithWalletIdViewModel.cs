namespace MoneySaver.Web.Models.Records
{
    using System;
    using System.Collections.Generic;

    public class RecordsWithWalletIdViewModel
    {
        public int WalletId { get; set; }

        public string Wallet { get; set; }

        public string SearchTerm { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<AllRecordsByWalletViewModel> Records { get; set; }
    }
}

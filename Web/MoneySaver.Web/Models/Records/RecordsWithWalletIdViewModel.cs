namespace MoneySaver.Web.Models.Records
{
    using System.Collections.Generic;

    public class RecordsWithWalletIdViewModel
    {
        public int WalletId { get; set; }
        public string Wallet { get; set; }

        public IEnumerable<AllRecordsByWalletViewModel> Records { get; set; }
    }
}

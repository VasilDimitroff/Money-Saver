namespace MoneySaver.Web.ViewModels.Wallets
{
    using System;

    using MoneySaver.Web.ViewModels.Records.Enums;

    public class WalletSearchResultSingleRecordViewModel
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string Category { get; set; }

        public BadgeColor BadgeColor { get; set; }

        public int CategoryId { get; set; }

        public RecordTypeInputModel Type { get; set; }

        public string Wallet { get; set; }

        public string CreatedOn { get; set; }

        public string ModifiedOn { get; set; }

        public string Currency { get; set; }
    }
}

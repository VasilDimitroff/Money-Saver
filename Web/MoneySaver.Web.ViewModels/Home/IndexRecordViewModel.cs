namespace MoneySaver.Web.ViewModels.Home
{
    using MoneySaver.Web.ViewModels.Records.Enums;

    public class IndexRecordViewModel
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public BadgeColor CategoryBadgeColor { get; set; }

        public RecordTypeInputModel Type { get; set; }

        public string CurrencyCode { get; set; }

        public decimal Amount { get; set; }

        public string CreatedOn { get; set; }

        public int WalletId { get; set; }

        public string WalletName { get; set; }
    }
}
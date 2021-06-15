namespace MoneySaver.Web.ViewModels.Wallets
{
    using MoneySaver.Web.ViewModels.Records.Enums;

    public class WalletDetailsRecordViewModel
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public BadgeColor CategoryBadgeColor { get; set; }

        public decimal Amount { get; set; }

        public string CreatedOn { get; set; }
    }
}

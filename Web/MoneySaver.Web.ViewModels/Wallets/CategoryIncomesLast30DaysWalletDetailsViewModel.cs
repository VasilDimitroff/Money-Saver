namespace MoneySaver.Web.ViewModels.Wallets
{
    using MoneySaver.Web.ViewModels.Records.Enums;

    public class CategoryIncomesLast30DaysWalletDetailsViewModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public BadgeColor BadgeColor { get; set; }

        public decimal TotalIncomesLast30days { get; set; }

        public int TotalIncomeRecordsLast30Days { get; set; }

    }
}

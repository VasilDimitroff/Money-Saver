namespace MoneySaver.Web.ViewModels.Wallets
{
    using MoneySaver.Web.ViewModels.Records.Enums;

    public class CategoryExpensesLast30DaysWalletDetailsViewModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public BadgeColor BadgeColor { get; set; }

        public decimal TotalExpensesLast30Days { get; set; }

        public int TotalExpenseRecordsLast30Days { get; set; }
    }
}

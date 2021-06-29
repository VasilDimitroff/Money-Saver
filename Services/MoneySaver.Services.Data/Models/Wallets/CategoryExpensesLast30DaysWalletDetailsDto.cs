namespace MoneySaver.Services.Data.Models.Wallets
{
    using MoneySaver.Data.Models.Enums;

    public class CategoryExpensesLast30DaysWalletDetailsDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public BadgeColor BadgeColor { get; set; }

        public decimal TotalExpensesLast30Days { get; set; }

        public int TotalExpenseRecordsLast30Days { get; set; }
    }
}

namespace MoneySaver.Services.Data.Models.Wallets
{
    using MoneySaver.Data.Models.Enums;

    public class CategoryIncomesLast30DaysWalletDetailsDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public BadgeColor BadgeColor { get; set; }

        public decimal TotalIncomesLast30Days { get; set; }

        public int TotalIncomeRecordsLast30Days { get; set; }
    }
}

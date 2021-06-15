using MoneySaver.Data.Models.Enums;

namespace MoneySaver.Services.Data.Models.Wallets
{
    public class WalletDetailsCategoryDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public BadgeColor BadgeColor { get; set; }

        public decimal TotalIncomes { get; set; }

        public decimal TotalExpenses { get; set; }

        public int RecordsCount { get; set; }
    }
}

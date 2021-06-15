namespace MoneySaver.Web.ViewModels.Wallets
{
    using MoneySaver.Web.ViewModels.Records.Enums;

    public class WalletDetailsCategoryViewModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public BadgeColor BadgeColor { get; set; }

        public decimal TotalIncomes { get; set; }

        public decimal TotalExpenses { get; set; }

        public int RecordsCount { get; set; }
    }
}

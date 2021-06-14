namespace MoneySaver.Web.Models.Wallets
{
    public class WalletDetailsCategoryViewModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public decimal TotalIncomes { get; set; }

        public decimal TotalExpenses { get; set; }

        public int RecordsCount { get; set; }
    }
}

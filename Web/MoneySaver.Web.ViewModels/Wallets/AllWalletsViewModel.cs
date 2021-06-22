namespace MoneySaver.Web.ViewModels.Wallets
{
    public class AllWalletsViewModel
    {
        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public string Currency { get; set; }

        public decimal TotalIncomes { get; set; }

        public decimal CurrentBalance { get; set; }

        public decimal TotalExpenses { get; set; }
    }
}

namespace MoneySaver.Services.Data.Models.Wallets
{
    public class AllWalletsDto
    {
        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public decimal TotalIncomes { get; set; }

        public decimal CurrentBalance { get; set; }

        public decimal TotalExpenses { get; set; }
    }
}

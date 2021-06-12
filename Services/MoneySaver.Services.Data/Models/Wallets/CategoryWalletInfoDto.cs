namespace MoneySaver.Services.Data.Models.Wallets
{
    public class CategoryWalletInfoDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int RecordsCount { get; set; }

        public decimal TotalExpensesAmount { get; set; }

        public decimal TotalIncomesAmount { get; set; }
    }
}

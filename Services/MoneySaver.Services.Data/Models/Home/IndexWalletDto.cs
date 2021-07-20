namespace MoneySaver.Services.Data.Models.Home
{
    public class IndexWalletDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CurrencyCode { get; set; }

        public decimal Amount { get; set; }
    }
}

namespace MoneySaver.Services.Data.Models.Home
{
    public class IndexInvestmentWalletDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CurrencyCode { get; set; }

        public decimal TotalBuyTradesAmount { get; set; }

        public decimal TotalSellTradesAmount { get; set; }
    }
}

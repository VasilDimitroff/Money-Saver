namespace MoneySaver.Services.Data.Models
{
    public class BudgetInfoDto
    {
        public string Name { get; set; }

        public string Wallet { get; set; }

        public decimal Amount { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }
}

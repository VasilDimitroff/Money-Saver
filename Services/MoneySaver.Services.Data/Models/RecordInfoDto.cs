namespace MoneySaver.Services.Data.Models
{
    public class RecordInfoDto
    {
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string Category { get; set; }

        public string Type { get; set; }

        public string Wallet { get; set; }
    }
}

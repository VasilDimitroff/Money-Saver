namespace MoneySaver.Services.Data.Models.Wallets
{
    using System;

    public class WalletDetailsRecordDto
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

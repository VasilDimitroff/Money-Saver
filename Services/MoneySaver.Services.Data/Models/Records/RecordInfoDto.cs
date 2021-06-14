namespace MoneySaver.Services.Data.Models.Records
{
    using MoneySaver.Data.Models.Enums;
    using System;

    public class RecordInfoDto
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string Category { get; set; }

        public int CategoryId { get; set; }

        public BadgeColor BadgeColor { get; set; }

        public string Type { get; set; }

        public string Wallet { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string Currency { get; set; }
    }
}

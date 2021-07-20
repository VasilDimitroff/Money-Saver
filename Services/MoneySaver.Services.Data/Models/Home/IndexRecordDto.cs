namespace MoneySaver.Services.Data.Models.Home
{
    using System;

    using MoneySaver.Data.Models.Enums;

    public class IndexRecordDto
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public BadgeColor CategoryBadgeColor { get; set; }

        public RecordType Type { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedOn { get; set; }

        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public string CurrencyCode { get; set; }
    }
}

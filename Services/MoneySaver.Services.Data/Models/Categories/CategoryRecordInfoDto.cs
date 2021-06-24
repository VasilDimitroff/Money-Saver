namespace MoneySaver.Services.Data.Models.Categories
{
    using System;

    using MoneySaver.Data.Models.Enums;

    public class CategoryRecordInfoDto
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public RecordType Type { get; set; }

        public DateTime CreatedOn { get; set; }

    }
}

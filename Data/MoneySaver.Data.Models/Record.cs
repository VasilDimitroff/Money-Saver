namespace MoneySaver.Data.Models
{
    using System;

    using MoneySaver.Data.Models.Enums;

    public class Record
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public RecordType Type { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

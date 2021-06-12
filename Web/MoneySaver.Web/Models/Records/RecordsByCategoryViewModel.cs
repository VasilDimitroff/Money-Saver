namespace MoneySaver.Web.Models.Records
{
    using MoneySaver.Web.Models.Records.Enums;
    using System;

    public class RecordsByCategoryViewModel
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public RecordTypeInputModel Type { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

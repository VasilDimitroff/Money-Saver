using MoneySaver.Web.Models.Records.Enums;
using System;
namespace MoneySaver.Web.Models.Records
{
    public class AllRecordsByWalletViewModel
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string Category { get; set; }

        public int CategoryId { get; set; }

        public RecordTypeInputModel Type { get; set; }

        public string Wallet { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Currency { get; set; }
    }
}

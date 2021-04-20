using MoneySaver.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.Data.Models
{
    public class Record
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public RecordType Type { get; set; }

        public int WalletId { get; set; }

        public virtual Wallet Wallet { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}

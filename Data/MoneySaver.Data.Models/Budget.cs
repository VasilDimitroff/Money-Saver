using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.Data.Models
{
    public class Budget
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WalletId { get; set; }
        public virtual Wallet Wallet { get; set; }
        public decimal Amount { get; set; }
    }
}

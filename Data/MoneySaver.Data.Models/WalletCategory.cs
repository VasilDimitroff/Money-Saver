using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.Data.Models
{
    public class WalletCategory
    {
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public int WalletId { get; set; }

        public Wallet Wallet { get; set; }
    }
}

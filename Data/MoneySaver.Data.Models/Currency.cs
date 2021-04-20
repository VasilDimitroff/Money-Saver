using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.Data.Models
{
    public class Currency
    {
        public Currency()
        {
            this.Wallets = new HashSet<Wallet>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}

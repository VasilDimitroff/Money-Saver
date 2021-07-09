namespace MoneySaver.Data.Models
{
    using System.Collections.Generic;

    public class Currency
    {
        public Currency()
        {
            this.Wallets = new HashSet<Wallet>();
            this.Trades = new HashSet<UserTrade>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public virtual ICollection<Wallet> Wallets { get; set; }

        public virtual ICollection<UserTrade> Trades { get; set; }
    }
}

namespace MoneySaver.Data.Models
{
    using System.Collections.Generic;

    public class Currency
    {
        public Currency()
        {
            this.Wallets = new HashSet<Wallet>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}

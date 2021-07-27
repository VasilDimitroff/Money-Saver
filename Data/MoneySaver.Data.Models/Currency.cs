namespace MoneySaver.Data.Models
{
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;

    public class Currency
    {
        public Currency()
        {
            this.Wallets = new HashSet<Wallet>();
            this.InvestmentWallets = new HashSet<InvestmentWallet>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(10)]
        public string Code { get; set; }

        public virtual ICollection<Wallet> Wallets { get; set; }

        public virtual ICollection<InvestmentWallet> InvestmentWallets { get; set; }
    }
}

namespace MoneySaver.Data.Models
{
    using System.Collections.Generic;

    using MoneySaver.Data.Common.Models;

    public class Wallet
    {
        public Wallet()
        {
            this.Budgets = new HashSet<Budget>();
            this.Categories = new HashSet<Category>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public decimal MoneyAmount { get; set; }

        public int CurrencyId { get; set; }

        public virtual Currency Currency { get; set; }

        public virtual ICollection<Budget> Budgets { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
}

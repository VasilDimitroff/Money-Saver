using System;
using System.Collections.Generic;

namespace MoneySaver.Data.Models
{
    public class Wallet
    {
        public Wallet()
        {
            this.Budgets = new HashSet<Budget>();
            this.Records = new HashSet<Record>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public decimal MoneyAmount { get; set; }

        public int CurrencyId { get; set; }

        public virtual Currency Currency { get; set; }

        public virtual ICollection<Budget> Budgets { get; set; }

        public virtual ICollection<Record> Records { get; set; }
    }
}

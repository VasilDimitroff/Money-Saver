namespace MoneySaver.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Data.Common.Models;

    public class Wallet : BaseDeletableModel<int>
    {
        public Wallet()
        {
            this.Budgets = new HashSet<Budget>();
            this.Categories = new HashSet<Category>();
        }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        public decimal MoneyAmount { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        public virtual Currency Currency { get; set; }

        public virtual ICollection<Budget> Budgets { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
}

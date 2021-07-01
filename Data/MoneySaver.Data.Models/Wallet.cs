namespace MoneySaver.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Data.Common.Models;

    public class Wallet : BaseModel<int>
    {
        public Wallet()
        {
            this.Categories = new HashSet<Category>();
        }

        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        public decimal MoneyAmount { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        public virtual Currency Currency { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
}

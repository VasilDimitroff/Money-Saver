namespace MoneySaver.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Data.Common.Models;
    using MoneySaver.Data.Models.Enums;

    public class Category : BaseModel<int>
    {
        public Category()
        {
            this.Records = new HashSet<Record>();
        }

        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public int WalletId { get; set; }

        public virtual Wallet Wallet { get; set; }

        [Required]
        public BadgeColor BadgeColor { get; set; }

        public virtual ICollection<Record> Records { get; set; }
    }
}

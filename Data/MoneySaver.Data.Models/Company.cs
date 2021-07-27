namespace MoneySaver.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Data.Common.Models;

    public class Company : BaseDeletableModel<string>
    {
        public Company()
        {
            this.Trades = new HashSet<Trade>();
        }

        [Required]
        [MaxLength(10)]
        public string Ticker { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public virtual ICollection<Trade> Trades { get; set; }
    }
}

namespace MoneySaver.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Data.Common.Models;

    public class Company
    {
        public Company()
        {
            this.Trades = new HashSet<Trade>();
        }

        [Key]
        [Required]
        public string Ticker { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public virtual ICollection<Trade> Trades { get; set; }
    }
}

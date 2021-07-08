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
            this.Stocks = new HashSet<Stock>();
        }

        [Key]
        [Required]
        public string Ticker { get; set; }

        public int Name { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public virtual ICollection<Stock> Stocks { get; set; }
    }
}
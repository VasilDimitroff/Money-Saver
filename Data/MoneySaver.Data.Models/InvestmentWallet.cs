using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using MoneySaver.Data.Common.Models;

namespace MoneySaver.Data.Models
{
    public class InvestmentWallet : BaseModel<int>
    {
        public InvestmentWallet()
        {
            this.Trades = new HashSet<Trade>();
        }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        public virtual Currency Currency { get; set; }

        public ICollection<Trade> Trades { get; set; }
    }
}

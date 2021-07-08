using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MoneySaver.Data.Common.Models;

namespace MoneySaver.Data.Models
{
    public class Stock : BaseModel<int>
    {
        public Stock()
        {
            this.Trades = new HashSet<UserTrade>();
        }

        [Required]
        public decimal Price { get; set; }

        [Required]
        [ForeignKey(nameof(Company))]
        public string CompanyTicker { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<UserTrade> Trades { get; set; }
    }
}

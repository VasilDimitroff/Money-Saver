using MoneySaver.Data.Common.Models;
using System.Collections.Generic;

namespace MoneySaver.Data.Models
{
    public class Stock : BaseDeletableModel<int>
    {
        public Stock()
        {
            this.Trades = new HashSet<UserTrade>();
        }

        public int Id { get; set; }

        public decimal Price { get; set; }

        public string CompanyName { get; set; }

        public virtual ICollection<UserTrade> Trades { get; set; }
    }
}

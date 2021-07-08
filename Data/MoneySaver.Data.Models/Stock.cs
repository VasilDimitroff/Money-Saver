using System.Collections.Generic;

using MoneySaver.Data.Common.Models;

namespace MoneySaver.Data.Models
{
    public class Stock : BaseDeletableModel<int>
    {
        public Stock()
        {
            this.Trades = new HashSet<UserTrade>();
        }

        public decimal Price { get; set; }

        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<UserTrade> Trades { get; set; }
    }
}

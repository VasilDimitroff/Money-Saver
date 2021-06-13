using MoneySaver.Data.Common.Models;
using System;
using System.Collections.Generic;

namespace MoneySaver.Data.Models
{
    public class Shoplist : BaseDeletableModel<int>
    {
        public Shoplist()
        {
            this.ProductsShoplist = new HashSet<ProductShoplist>();
        }

        public string Name { get; set; }

        public bool IsClosed { get; set; }

        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<ProductShoplist> ProductsShoplist { get; set; }
    }
}

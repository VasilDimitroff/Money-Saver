using System;
using System.Collections.Generic;

namespace MoneySaver.Data.Models
{
    public class Shoplist
    {
        public Shoplist()
        {
            this.ProductsShoplist = new HashSet<ProductShoplist>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsClosed { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<ProductShoplist> ProductsShoplist { get; set; }
    }
}

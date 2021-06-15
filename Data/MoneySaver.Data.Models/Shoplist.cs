namespace MoneySaver.Data.Models
{

    using System;
    using System.Collections.Generic;

    using MoneySaver.Data.Common.Models;
    using MoneySaver.Data.Models.Enums;

    public class Shoplist : BaseModel<int>
    {
        public Shoplist()
        {
            this.ProductsShoplist = new HashSet<ProductShoplist>();
        }

        public string Name { get; set; }

        public StatusType Status { get; set; }

        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<ProductShoplist> ProductsShoplist { get; set; }
    }
}

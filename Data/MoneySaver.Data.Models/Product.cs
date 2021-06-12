using System.Collections.Generic;

namespace MoneySaver.Data.Models
{
    public class Product
    {
        public Product()
        {
            this.ProductShoplists = new HashSet<ProductShoplist>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<ProductShoplist> ProductShoplists { get; set; }
    }
}

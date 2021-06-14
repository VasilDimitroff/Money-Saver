namespace MoneySaver.Data.Models
{
    using System.Collections.Generic;

    using MoneySaver.Data.Common.Models;

    public class Category : BaseDeletableModel<int>
    {
        public Category()
        {
            this.Records = new HashSet<Record>();
            this.Products = new HashSet<Product>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int WalletId { get; set; }

        public virtual Wallet Wallet { get; set; }

        public virtual ICollection<Record> Records { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}

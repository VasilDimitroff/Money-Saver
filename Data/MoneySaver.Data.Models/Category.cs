namespace MoneySaver.Data.Models
{
    using System;
    using System.Collections.Generic;

    using MoneySaver.Data.Common.Models;
    using MoneySaver.Data.Models.Enums;

    public class Category : BaseModel<int>
    {
        public Category()
        {
            this.Records = new HashSet<Record>();
            this.Products = new HashSet<ToDoItem>();
        }

        public string Name { get; set; }

        public int WalletId { get; set; }

        public virtual Wallet Wallet { get; set; }

        public BadgeColor BadgeColor { get; set; }

        public virtual ICollection<Record> Records { get; set; }

        public virtual ICollection<ToDoItem> Products { get; set; }
    }
}

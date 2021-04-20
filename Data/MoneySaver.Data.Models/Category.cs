using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.Data.Models
{
    public class Category
    {
        public Category()
        {
            this.Records = new HashSet<Record>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Record> Records { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.Web.Models
{
    public class RecordViewModel
    {
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string Category { get; set; }

        public string Type { get; set; }

        public string Wallet { get; set; }
    }
}

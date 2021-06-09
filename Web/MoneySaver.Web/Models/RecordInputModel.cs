namespace MoneySaver.Web.Models
{
    using MoneySaver.Data.Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class RecordInputModel
    {
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public int CategoryId { get; set; }

        public RecordType Type { get; set; }

        public int WalletId { get; set; }

        public string CreatedOn { get; set; }
    }
}

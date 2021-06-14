using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.Web.ViewModels.Records
{
    public class EditRecordInputModel
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public int WalletId { get; set; }

        public string Type { get; set; }

        public decimal OldAmount { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

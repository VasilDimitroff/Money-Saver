using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.Web.ViewModels.Wallets
{
    public class WalletDetailsRecordViewModel
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public decimal Amount { get; set; }

        public string CreatedOn { get; set; }
    }
}

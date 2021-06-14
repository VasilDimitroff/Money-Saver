namespace MoneySaver.Web.Models.Wallets
{
    using System.Collections.Generic;
    using System.Linq;

    public class WalletDetailsViewModel
    {
        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public string Currency { get; set; }

        public decimal TotalWalletExpenses => this.Categories.Sum(c => c.TotalExpenses);

        public decimal TotalWalletIncomes => this.Categories.Sum(c => c.TotalIncomes);

        public decimal CurrentBalance { get; set; }

        public IEnumerable<WalletDetailsCategoryViewModel> Categories { get; set; }

        public IEnumerable<WalletDetailsRecordViewModel> Records { get; set; }
    }
}

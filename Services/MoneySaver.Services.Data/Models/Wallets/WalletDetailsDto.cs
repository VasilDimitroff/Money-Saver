namespace MoneySaver.Services.Data.Models.Wallets
{
    using System.Collections.Generic;
    using System.Linq;

    public class WalletDetailsDto
    {
        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public string Currency { get; set; }

        public decimal TotalWalletExpenses => this.Categories.Sum(c => c.TotalExpenses);

        public decimal TotalWalletIncomes => this.Categories.Sum(c => c.TotalIncomes);

        public decimal CurrentBalance { get; set; }

        public decimal TotalWalletExpensesLast30Days { get; set; }

        public decimal TotalWalletIncomesLast30Days { get; set; }

        public int TotalRecordsCountLast30Days { get; set; }

        public IEnumerable<WalletDetailsCategoryDto> Categories { get; set; }

        public IEnumerable<WalletDetailsRecordDto> Records { get; set; }
    }
}

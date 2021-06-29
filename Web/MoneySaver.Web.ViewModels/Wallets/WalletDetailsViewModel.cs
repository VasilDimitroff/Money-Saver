namespace MoneySaver.Web.ViewModels.Wallets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MoneySaver.Web.ViewModels.Categories;

    public class WalletDetailsViewModel
    {
        public WalletDetailsViewModel()
        {
            this.MonthIncomes = new HashSet<CategoryIncomesLast30DaysWalletDetailsViewModel>();
            this.MonthExpenses = new HashSet<CategoryExpensesLast30DaysWalletDetailsViewModel>();
            this.Categories = new HashSet<WalletDetailsCategoryViewModel>();
            this.Records = new HashSet<WalletDetailsRecordViewModel>();
        }

        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public string Currency { get; set; }

        public decimal TotalWalletExpenses => this.Categories.Sum(c => c.TotalExpenses);

        public decimal TotalWalletIncomes => this.Categories.Sum(c => c.TotalIncomes);

        public decimal CurrentBalance { get; set; }

        public decimal TotalWalletBalanceLast30Days
            => this.TotalWalletIncomesLast30Days - Math.Abs(this.TotalWalletExpensesLast30Days);

        public decimal TotalWalletExpensesLast30Days { get; set; }

        public decimal TotalWalletIncomesLast30Days { get; set; }

        public int TotalRecordsCountLast30Days { get; set; }

        public IEnumerable<WalletDetailsCategoryViewModel> Categories { get; set; }

        public IEnumerable<WalletDetailsRecordViewModel> Records { get; set; }

        public IEnumerable<CategoryIncomesLast30DaysWalletDetailsViewModel> MonthIncomes { get; set; }

        public IEnumerable<CategoryExpensesLast30DaysWalletDetailsViewModel> MonthExpenses { get; set; }
    }
}

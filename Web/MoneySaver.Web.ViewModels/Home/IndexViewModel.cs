namespace MoneySaver.Web.ViewModels.Home
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MoneySaver.Web.ViewModels.Records.Enums;

    public class IndexViewModel
    {
        public IndexViewModel()
        {
            this.CategoriesLast30DaysIncomes = new HashSet<AccountCategoryIncomesLast30DaysViewModel>();
            this.CategoriesLast30DaysExpenses = new HashSet<AccountCategoryExpensesLast30DaysViewModel>();
            this.ActiveToDoLists = new HashSet<IndexListViewModel>();
            this.Wallets = new HashSet<IndexWalletViewModel>();
            this.InvestmentWallets = new HashSet<IndexInvestmentWalletViewModel>();
            this.AccountRecords = new HashSet<IndexRecordViewModel>();
            this.AccountTrades = new HashSet<IndexTradeViewModel>();
            this.AccountHoldings = new HashSet<IndexCompanyHoldingsViewModel>();
            this.AccountCategories = new HashSet<IndexCategoriesSummaryViewModel>();
        }

        public decimal TotalAccountExpenses => this.AccountRecords
            .Where(r => r.Type == RecordTypeInputModel.Expense)
            .Count();

        public decimal TotalAccountIncomes => this.AccountRecords
           .Where(r => r.Type == RecordTypeInputModel.Income)
           .Count();

        public int TotalAccountTrades => this.AccountTrades.Count();

        public int TotalStockHoldings => this.AccountHoldings.Sum(h => h.StocksHoldings);

        public decimal TotalAccountIncomesLast30Days => this.CategoriesLast30DaysIncomes.Sum(c => c.TotalIncomesLast30days);

        public decimal TotalAccountExpensesLast30Days => this.CategoriesLast30DaysExpenses.Sum(c => c.TotalExpensesLast30Days);

        public ICollection<AccountCategoryIncomesLast30DaysViewModel> CategoriesLast30DaysIncomes { get; set; }

        public ICollection<AccountCategoryExpensesLast30DaysViewModel> CategoriesLast30DaysExpenses { get; set; }

        public ICollection<IndexListViewModel> ActiveToDoLists { get; set; }

        public ICollection<IndexWalletViewModel> Wallets { get; set; }

        public IEnumerable<IndexInvestmentWalletViewModel> InvestmentWallets { get; set; }

        public IEnumerable<IndexRecordViewModel> AccountRecords { get; set; }

        public IEnumerable<IndexTradeViewModel> AccountTrades { get; set; }

        public IEnumerable<IndexCompanyHoldingsViewModel> AccountHoldings { get; set; }

        public IEnumerable<IndexCategoriesSummaryViewModel> AccountCategories { get; set; }
    }
}

namespace MoneySaver.Web.ViewModels.Home
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class IndexViewModel
    {
        public IndexViewModel()
        {
            this.CategoriesLast30DaysIncomes = new HashSet<AccountCategoryIncomesLast30DaysViewModel>();
            this.CategoriesLast30DaysExpenses = new HashSet<AccountCategoryExpensesLast30DaysViewModel>();
            this.ActiveToDoLists = new HashSet<IndexListViewModel>();
            this.Wallets = new HashSet<IndexWalletViewModel>();
        }

       // public decimal TotalAccountExpenses => this.CategoriesLast30DaysExpenses.Sum(c => c.TotalExpenses);

        public decimal TotalAccountIncomesLast30Days => this.CategoriesLast30DaysIncomes.Sum(c => c.TotalIncomesLast30days);

        public decimal TotalAccountExpensesLast30Days => this.CategoriesLast30DaysExpenses.Sum(c => c.TotalExpensesLast30Days);

        public ICollection<AccountCategoryIncomesLast30DaysViewModel> CategoriesLast30DaysIncomes { get; set; }

        public ICollection<AccountCategoryExpensesLast30DaysViewModel> CategoriesLast30DaysExpenses { get; set; }

        public ICollection<IndexListViewModel> ActiveToDoLists { get; set; }

        public ICollection<IndexWalletViewModel> Wallets { get; set; }
    }
}

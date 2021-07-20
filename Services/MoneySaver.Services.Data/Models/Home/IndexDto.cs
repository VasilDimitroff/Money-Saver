namespace MoneySaver.Services.Data.Models.Home
{
    using System;
    using System.Collections.Generic;

    public class IndexDto
    {
        public IndexDto()
        {
            this.CategoriesLast30DaysExpenses = new HashSet<AccountCategoryExpensesLast30DaysDto>();
            this.CategoriesLast30DaysIncomes = new HashSet<AccountCategoryIncomesLast30DaysDto>();
            this.ActiveToDoLists = new HashSet<IndexListDto>();
            this.Wallets = new HashSet<IndexWalletDto>();
        }

        public IEnumerable<AccountCategoryIncomesLast30DaysDto> CategoriesLast30DaysIncomes { get; set; }

        public IEnumerable<AccountCategoryExpensesLast30DaysDto> CategoriesLast30DaysExpenses { get; set; }

        public IEnumerable<IndexListDto> ActiveToDoLists { get; set; }

        public IEnumerable<IndexWalletDto> Wallets { get; set; }
    }
}

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
            this.InvestmentWallets = new HashSet<IndexInvestmentWalletDto>();
            this.AccountRecords = new HashSet<IndexRecordDto>();
            this.AccountTrades = new HashSet<IndexTradeDto>();
        }

        public IEnumerable<AccountCategoryIncomesLast30DaysDto> CategoriesLast30DaysIncomes { get; set; }

        public IEnumerable<AccountCategoryExpensesLast30DaysDto> CategoriesLast30DaysExpenses { get; set; }

        public IEnumerable<IndexListDto> ActiveToDoLists { get; set; }

        public IEnumerable<IndexWalletDto> Wallets { get; set; }

        public IEnumerable<IndexInvestmentWalletDto> InvestmentWallets { get; set; }

        public IEnumerable<IndexRecordDto> AccountRecords { get; set; }

        public IEnumerable<IndexTradeDto> AccountTrades { get; set; }
    }
}

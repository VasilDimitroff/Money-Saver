namespace MoneySaver.Web.ViewModels.Wallets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MoneySaver.Web.ViewModels.Categories;

    public class StatisticsWalletViewModel
    {
        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public string Currency { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal Incomes { get; set; }

        public decimal Outcomes { get; set; }

        public string ModifiedOn { get; set; }

        public int RecordsCount => this.Categories.Sum(c => c.TotalRecordsCount);

        public ICollection<CategoryStatisticsViewModel> Categories { get; set; }
    }
}

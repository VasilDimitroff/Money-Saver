namespace MoneySaver.Web.Models.Statistics
{
    using System;
    using System.Collections.Generic;

    using MoneySaver.Web.Models.Categories;

    public class StatisticsWalletViewModel
    {
        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public IEnumerable<CategoryStatisticsViewModel> Categories { get; set; }
    }
}

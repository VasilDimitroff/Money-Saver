namespace MoneySaver.Web.ViewModels.Categories
{
    using System;
    using System.Collections.Generic;

    using MoneySaver.Web.ViewModels.Records;
    using MoneySaver.Web.ViewModels.Records.Enums;

    public class CategoryRecordsViewModel : SearchPagingViewModel
    {
        public string Category { get; set; }

        public int CategoryId { get; set; }

        public string Currency { get; set; }

        public string WalletName { get; set; }

        public BadgeColor BadgeColor { get; set; }

        public IEnumerable<RecordsByCategoryViewModel> Records { get; set; }
    }
}

namespace MoneySaver.Services.Data.Models.Categories
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using MoneySaver.Data.Models.Enums;

    public class AllRecordsInCategoryDto
    {
        public string Category { get; set; }

        public int CategoryId { get; set; }

        public string Currency { get; set; }

        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public BadgeColor BadgeColor { get; set; }

        public IEnumerable<CategoryRecordInfoDto> Records { get; set; }
    }
}

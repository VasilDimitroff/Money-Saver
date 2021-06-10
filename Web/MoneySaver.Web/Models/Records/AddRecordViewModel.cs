namespace MoneySaver.Web.Models.Records
{
    using System.Collections.Generic;

    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Web.Models.Categories;
    using MoneySaver.Web.Models.Records.Enums;

    public class AddRecordViewModel
    {
        public int CategoryId { get; set; }

        public string Description { get; set; }

        public string WalletName { get; set; }

        public int WalletId { get; set; }

        public decimal Amount { get; set; }

        public IEnumerable<CategoryNameIdViewModel> Categories { get; set; }

        public RecordTypeInputModel Type { get; set; }

    }
}

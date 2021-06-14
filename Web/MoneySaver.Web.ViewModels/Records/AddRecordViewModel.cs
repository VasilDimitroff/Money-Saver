namespace MoneySaver.Web.ViewModels.Records
{
    using System.Collections.Generic;
    using MoneySaver.Web.ViewModels.Categories;
    using MoneySaver.Web.ViewModels.Wallets;
    using MoneySaver.Web.ViewModels.Records.Enums;

    public class AddRecordViewModel
    {
        public int WalletId { get; set; }

        public int CategoryId { get; set; }

        public string Description { get; set; }

        public string WalletName { get; set; }

        public decimal Amount { get; set; }

        public IEnumerable<CategoryNameIdViewModel> Categories { get; set; }

        public RecordTypeInputModel Type { get; set; }

    }
}

namespace MoneySaver.Web.Models.Records
{
    using System.Collections.Generic;
    using MoneySaver.Web.Infrastructure.CustomValidations;
    using MoneySaver.Web.Models.Categories;
    using MoneySaver.Web.Models.Records.Enums;
    using MoneySaver.Web.Models.Wallets;

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
